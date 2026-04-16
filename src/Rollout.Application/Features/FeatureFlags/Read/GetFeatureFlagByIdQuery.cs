using FluentResults;
using MediatR;

namespace Rollout.Application.Features.FeatureFlags.Read;

/// <summary>
/// Query to retrieve a specific feature flag by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the flag.</param>
public sealed record GetFeatureFlagByIdQuery(Guid Id) : IRequest<Result<FeatureFlagDto>>;

