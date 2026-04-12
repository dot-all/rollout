namespace Rollout.Application.Features.FeatureFlags.Evaluation;

public sealed record EvaluationResponseDto(string Key, bool IsEnabledForUser);
