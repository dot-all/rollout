using FluentResults;
using MediatR;

namespace Rollout.Application.Features.FeatureFlags.Delete;

public sealed record DeleteFeatureFlagCommand(Guid Id) : IRequest<Result>;
