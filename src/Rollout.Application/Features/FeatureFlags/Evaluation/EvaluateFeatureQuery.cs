using FluentResults;
using MediatR;

namespace Rollout.Application.Features.FeatureFlags.Evaluation;

public sealed record EvaluateFeatureQuery(string Key, string UserId) : IRequest<Result<EvaluationResponseDto>>;
