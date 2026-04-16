using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rollout.Domain.Entities;

namespace Rollout.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database schema for the <see cref="FeatureFlag"/> entity.
/// </summary>
public sealed class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
{
    public void Configure(EntityTypeBuilder<FeatureFlag> builder)
    {
        builder.ToTable("FeatureFlags");

        builder.HasKey(flag => flag.Id);
        
        // Use client-generated GUIDs for the primary key.
        builder.Property(flag => flag.Id)
            .ValueGeneratedNever();

        builder.Property(flag => flag.Key)
            .IsRequired()
            .HasMaxLength(100);

        // Ensure business key uniqueness at the database level.
        builder.HasIndex(flag => flag.Key)
            .IsUnique();

        builder.Property(flag => flag.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(flag => flag.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(flag => flag.IsEnabled)
            .IsRequired();

        builder.Property(flag => flag.RolloutPercentage)
            .IsRequired();

        // Targeting rules are stored as a JSON column to simplify the schema while maintaining extensibility.
        // This is preferred over a separate normalized table since rules are always loaded alongside the flag.
        builder.OwnsMany(flag => flag.TargetingRules, owned =>
        {
            owned.ToJson();
            
            owned.Property(rule => rule.Attribute)
                .IsRequired()
                .HasMaxLength(100);

            owned.Property(rule => rule.Operator)
                .IsRequired()
                .HasMaxLength(50);

            owned.Property(rule => rule.Value)
                .IsRequired()
                .HasMaxLength(500);
        });
    }
}

