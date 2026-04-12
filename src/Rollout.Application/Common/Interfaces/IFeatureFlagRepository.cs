using Rollout.Domain.Entities;

namespace Rollout.Application.Common.Interfaces;

public interface IFeatureFlagRepository
{
    Task<bool> ExistsByKeyAsync(string key, CancellationToken cancellationToken);
    Task AddAsync(FeatureFlag featureFlag, CancellationToken cancellationToken);
}
