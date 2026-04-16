using Rollout.Domain.Entities;

namespace Rollout.Application.Features.FeatureFlags;

/// <summary>
/// Data transfer object representing a feature flag's full configuration.
/// </summary>
public sealed record FeatureFlagDto(
    Guid Id,
    string Key,
    string Name,
    string Description,
    bool IsEnabled,
    int RolloutPercentage,
    IReadOnlyList<TargetingRuleDto> TargetingRules)
{
    /// <summary>
    /// Maps a <see cref="FeatureFlag"/> domain entity to a <see cref="FeatureFlagDto"/>.
    /// </summary>
    public static FeatureFlagDto FromEntity(FeatureFlag featureFlag)
        => new(
            featureFlag.Id,
            featureFlag.Key,
            featureFlag.Name,
            featureFlag.Description,
            featureFlag.IsEnabled,
            featureFlag.RolloutPercentage,
            featureFlag.TargetingRules.Select(TargetingRuleDto.FromDomain).ToList());
}

