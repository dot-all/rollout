using Microsoft.EntityFrameworkCore;
using Rollout.Application.Common.Interfaces;
using Rollout.Domain.Entities;

namespace Rollout.Infrastructure.Persistence.Repositories;

public sealed class FeatureFlagRepository : IFeatureFlagRepository
{
    private readonly RolloutDbContext _dbContext;

    public FeatureFlagRepository(RolloutDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(FeatureFlag featureFlag, CancellationToken cancellationToken)
    {
        await _dbContext.FeatureFlags.AddAsync(featureFlag, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByKeyAsync(string key, CancellationToken cancellationToken)
    {
        return _dbContext.FeatureFlags.AnyAsync(entity => entity.Key == key, cancellationToken);
    }

    public async Task<FeatureFlag?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.FeatureFlags.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<FeatureFlag?> GetByKeyAsync(string key, CancellationToken cancellationToken)
    {
        return await _dbContext.FeatureFlags
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Key == key, cancellationToken);
    }

    public async Task<IEnumerable<FeatureFlag>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.FeatureFlags
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(FeatureFlag featureFlag, CancellationToken cancellationToken)
    {
        _dbContext.FeatureFlags.Update(featureFlag);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(FeatureFlag featureFlag, CancellationToken cancellationToken)
    {
        _dbContext.FeatureFlags.Remove(featureFlag);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
