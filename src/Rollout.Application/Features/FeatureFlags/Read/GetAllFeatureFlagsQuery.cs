using FluentResults;
using MediatR;

namespace Rollout.Application.Features.FeatureFlags.Read;

/// <summary>
/// Query to retrieve all configured feature flags.
/// </summary>
public sealed record GetAllFeatureFlagsQuery() : IRequest<Result<IEnumerable<FeatureFlagDto>>>;

