using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Rollout.Application.Common.Interfaces;
using Rollout.Domain.Entities;

namespace Rollout.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implements persistent storage for feature flags with an integrated distributed cache layer.
/// This implementation follows the Cache-Aside pattern to reduce database load during high-frequency evaluations.
/// </summary>
public sealed class FeatureFlagRepository : IFeatureFlagRepository
{
    private const string CacheKeyPrefix = "featureflag:";
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly RolloutDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public FeatureFlagRepository(RolloutDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task AddAsync(FeatureFlag featureFlag, CancellationToken cancellationToken)
    {
        await _dbContext.FeatureFlags.AddAsync(featureFlag, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        // Cache is not warmed immediately on creation as it might never be requested soon.
    }

    public Task<bool> ExistsByKeyAsync(string key, CancellationToken cancellationToken)
    {
        return _dbContext.FeatureFlags.AnyAsync(entity => entity.Key == key, cancellationToken);
    }

    public async Task<FeatureFlag?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Direct ID lookups skip the cache for now as evaluations usually happen by business Key.
        return await _dbContext.FeatureFlags.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<FeatureFlag?> GetByKeyAsync(string key, CancellationToken cancellationToken)
    {
        string cacheKey = BuildCacheKey(key);
        string? cachedValue = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedValue))
        {
            var cacheItem = JsonSerializer.Deserialize<CachedFeatureFlag>(cachedValue, SerializerOptions);
            if (cacheItem is not null)
            {
                // Rehydration from cache ensures the domain entity is correctly instantiated.
                return cacheItem.ToDomain();
            }
        }

        // Cache miss: Load from DB and populate cache.
        var featureFlag = await _dbContext.FeatureFlags
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Key == key, cancellationToken);

        if (featureFlag is not null)
        {
            await SetCacheAsync(featureFlag, cancellationToken);
        }

        return featureFlag;
    }

    public async Task<IEnumerable<FeatureFlag>> GetAllAsync(CancellationToken cancellationToken)
    {
        // Listing operations are always performed against the database to ensure consistency.
        return await _dbContext.FeatureFlags
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(FeatureFlag featureFlag, CancellationToken cancellationToken)
    {
        _dbContext.FeatureFlags.Update(featureFlag);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        // Invalidation over update is preferred over immediate re-caching to preserve eventual consistency.
        await RemoveCacheAsync(featureFlag.Key, cancellationToken);
    }

    public async Task DeleteAsync(FeatureFlag featureFlag, CancellationToken cancellationToken)
    {
        _dbContext.FeatureFlags.Remove(featureFlag);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await RemoveCacheAsync(featureFlag.Key, cancellationToken);
    }

    private static string BuildCacheKey(string key) => CacheKeyPrefix + key;

    private async Task SetCacheAsync(FeatureFlag featureFlag, CancellationToken cancellationToken)
    {
        var cacheItem = CachedFeatureFlag.FromDomain(featureFlag);
        string payload = JsonSerializer.Serialize(cacheItem, SerializerOptions);
        
        // Using default options; in a production scenario, we might want to specify absolute/sliding expiration.
        await _cache.SetStringAsync(BuildCacheKey(featureFlag.Key), payload, new DistributedCacheEntryOptions(), cancellationToken);
    }

    private Task RemoveCacheAsync(string key, CancellationToken cancellationToken)
    {
        return _cache.RemoveAsync(BuildCacheKey(key), cancellationToken);
    }

    /// <summary>
    /// Specialized DTO for cache persistence to avoid serializing complex domain behaviors.
    /// </summary>
    private sealed record CachedTargetingRule(string Attribute, string Operator, string Value)
    {
        public TargetingRule ToDomain() => new(Attribute, Operator, Value);
    }

    /// <summary>
    /// Specialized DTO for cache persistence.
    /// </summary>
    private sealed record CachedFeatureFlag(
        Guid Id,
        string Key,
        string Name,
        string Description,
        bool IsEnabled,
        int RolloutPercentage,
        List<CachedTargetingRule> TargetingRules)
    {
        /// <summary>
        /// Converts the cached representation back into a rich domain entity.
        /// </summary>
        public FeatureFlag ToDomain()
        {
            var featureFlag = FeatureFlag.Create(
                Key,
                Name,
                Description,
                IsEnabled,
                RolloutPercentage,
                TargetingRules.Select(rule => rule.ToDomain()));

            // Reflection is used to restore the private Id property. 
            // In a more complex architecture, we might use a dedicated rehydration factory or constructor.
            var idProperty = typeof(FeatureFlag).GetProperty("Id", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            idProperty?.SetValue(featureFlag, Id);

            return featureFlag;
        }

        public static CachedFeatureFlag FromDomain(FeatureFlag featureFlag)
            => new(
                featureFlag.Id,
                featureFlag.Key,
                featureFlag.Name,
                featureFlag.Description,
                featureFlag.IsEnabled,
                featureFlag.RolloutPercentage,
                featureFlag.TargetingRules.Select(rule => new CachedTargetingRule(rule.Attribute, rule.Operator, rule.Value)).ToList());
    }
}

