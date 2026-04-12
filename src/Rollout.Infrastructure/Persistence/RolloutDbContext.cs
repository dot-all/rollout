using Microsoft.EntityFrameworkCore;
using Rollout.Domain.Entities;
using Rollout.Infrastructure.Persistence.Configurations;

namespace Rollout.Infrastructure.Persistence;

public sealed class RolloutDbContext : DbContext
{
    public RolloutDbContext(DbContextOptions<RolloutDbContext> options)
        : base(options)
    {
    }

    public DbSet<FeatureFlag> FeatureFlags { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FeatureFlagConfiguration());
    }
}
