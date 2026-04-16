using FluentResults;
using MediatR;

namespace Rollout.Application.Features.FeatureFlags.Evaluation;

/// <summary>
/// Query to evaluate a feature flag for a specific user context.
/// Returns whether the feature should be enabled or disabled for that user.
/// </summary>
/// <param name="Key">The unique business key of the feature flag.</param>
/// <param name="UserContext">The attributes of the user being evaluated.</param>
public sealed record EvaluateFeatureQuery(string Key, UserContextDto UserContext) : IRequest<Result<EvaluationResponseDto>>;

