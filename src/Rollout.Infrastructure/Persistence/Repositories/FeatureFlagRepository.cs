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
}
