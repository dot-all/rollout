using Rollout.Domain.Entities;

namespace Rollout.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for persistence operations related to feature flags.
/// </summary>
public interface IFeatureFlagRepository
{
    /// <summary>
    /// Checks if a feature flag with the specified unique key already exists.
    /// </summary>
    Task<bool> ExistsByKeyAsync(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Persists a new feature flag entity.
    /// </summary>
    Task AddAsync(FeatureFlag featureFlag, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a feature flag by its unique identifier.
    /// </summary>
    Task<FeatureFlag?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a feature flag by its unique business key.
    /// </summary>
    Task<FeatureFlag?> GetByKeyAsync(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all feature flags currently stored.
    /// </summary>
    Task<IEnumerable<FeatureFlag>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Persists changes to an existing feature flag entity.
    /// </summary>
    Task UpdateAsync(FeatureFlag featureFlag, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a feature flag entity from persistence.
    /// </summary>
    Task DeleteAsync(FeatureFlag featureFlag, CancellationToken cancellationToken);
}

