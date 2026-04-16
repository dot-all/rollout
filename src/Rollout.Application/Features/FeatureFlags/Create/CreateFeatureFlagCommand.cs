using FluentResults;
using MediatR;

namespace Rollout.Application.Features.FeatureFlags.Create;

/// <summary>
/// Command to create a new feature flag with optional targeting rules.
/// </summary>
/// <param name="Key">The unique identifier for the feature flag (e.g., 'new-checkout-v2').</param>
/// <param name="Name">A human-readable name for the feature flag.</param>
/// <param name="Description">A brief explanation of what the feature flag controls.</param>
/// <param name="IsEnabled">Initial active status of the flag.</param>
/// <param name="RolloutPercentage">Percentage of users who will see the feature (0-100).</param>
/// <param name="TargetingRules">Optional rules to restrict the feature to specific user segments.</param>
public sealed record CreateFeatureFlagCommand(
    string Key,
    string Name,
    string Description,
    bool IsEnabled,
    int RolloutPercentage,
    IEnumerable<TargetingRuleDto>? TargetingRules = null) : IRequest<Result<Guid>>;

