using Rollout.Domain.Entities;

namespace Rollout.Application.Common.Interfaces;

public interface IFeatureFlagRepository
{
    Task<bool> ExistsByKeyAsync(string key, CancellationToken cancellationToken);
    Task AddAsync(FeatureFlag featureFlag, CancellationToken cancellationToken);
    Task<FeatureFlag?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<FeatureFlag?> GetByKeyAsync(string key, CancellationToken cancellationToken);
    Task<IEnumerable<FeatureFlag>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdateAsync(FeatureFlag featureFlag, CancellationToken cancellationToken);
    Task DeleteAsync(FeatureFlag featureFlag, CancellationToken cancellationToken);
}
