using FluentResults;
using MediatR;

namespace Rollout.Application.Features.FeatureFlags.Update;

/// <summary>
/// Command to update the configuration of an existing feature flag.
/// </summary>
/// <param name="Id">The unique identifier of the flag to update.</param>
/// <param name="Name">The updated display name.</param>
/// <param name="Description">The updated description.</param>
/// <param name="IsEnabled">The updated active status.</param>
/// <param name="RolloutPercentage">The updated rollout percentage (0-100).</param>
/// <param name="TargetingRules">The updated set of targeting rules. If null, existing rules remain unchanged.</param>
public sealed record UpdateFeatureFlagCommand(
    Guid Id,
    string Name,
    string Description,
    bool IsEnabled,
    int RolloutPercentage,
    IEnumerable<TargetingRuleDto>? TargetingRules = null) : IRequest<Result>;

