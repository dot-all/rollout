using Microsoft.EntityFrameworkCore;
using Rollout.Domain.Entities;
using Rollout.Infrastructure.Persistence.Configurations;

namespace Rollout.Infrastructure.Persistence;

/// <summary>
/// The primary database context for the Rollout application.
/// </summary>
public sealed class RolloutDbContext : DbContext
{
    public RolloutDbContext(DbContextOptions<RolloutDbContext> options)
        : base(options)
    {
    }

    public DbSet<FeatureFlag> FeatureFlags { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Centralized configuration using IEntityTypeConfiguration to keep the DbContext clean.
        modelBuilder.ApplyConfiguration(new FeatureFlagConfiguration());
    }
}

