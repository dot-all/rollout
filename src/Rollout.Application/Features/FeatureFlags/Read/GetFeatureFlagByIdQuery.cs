using FluentResults;
using MediatR;

namespace Rollout.Application.Features.FeatureFlags.Read;

public sealed record GetFeatureFlagByIdQuery(Guid Id) : IRequest<Result<FeatureFlagDto>>;
