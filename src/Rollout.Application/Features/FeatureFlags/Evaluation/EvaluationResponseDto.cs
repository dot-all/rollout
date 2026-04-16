namespace Rollout.Application.Features.FeatureFlags.Evaluation;

/// <summary>
/// Represents the result of a feature evaluation.
/// </summary>
/// <param name="Key">The unique business key of the feature flag.</param>
/// <param name="IsEnabledForUser">Indicates if the feature is enabled for the evaluated user context.</param>
public sealed record EvaluationResponseDto(string Key, bool IsEnabledForUser);

