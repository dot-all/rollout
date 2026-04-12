using FluentResults;
using MediatR;

namespace Rollout.Application.Features.FeatureFlags.Read;

public sealed record GetAllFeatureFlagsQuery() : IRequest<Result<IEnumerable<FeatureFlagDto>>>;
