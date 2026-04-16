using FluentResults;
using MediatR;

namespace Rollout.Application.Features.FeatureFlags.Delete;

/// <summary>
/// Command to permanently remove a feature flag from the system.
/// </summary>
/// <param name="Id">The unique identifier of the flag to delete.</param>
public sealed record DeleteFeatureFlagCommand(Guid Id) : IRequest<Result>;

