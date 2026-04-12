using Rollout.Domain.Entities;

namespace Rollout.Application.Features.FeatureFlags;

public sealed record FeatureFlagDto(
    Guid Id,
    string Key,
    string Name,
    string Description,
    bool IsEnabled,
    int RolloutPercentage)
{
    public static FeatureFlagDto FromEntity(FeatureFlag featureFlag)
        => new(
            featureFlag.Id,
            featureFlag.Key,
            featureFlag.Name,
            featureFlag.Description,
            featureFlag.IsEnabled,
            featureFlag.RolloutPercentage);
}
