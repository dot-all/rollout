using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rollout.Domain.Entities;

namespace Rollout.Infrastructure.Persistence.Configurations;

public sealed class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
{
    public void Configure(EntityTypeBuilder<FeatureFlag> builder)
    {
        builder.ToTable("FeatureFlags");

        builder.HasKey(flag => flag.Id);
        builder.Property(flag => flag.Id)
            .ValueGeneratedNever();

        builder.Property(flag => flag.Key)
            .IsRequired()
            .HasMaxLength(100);

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
    }
}
