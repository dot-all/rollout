using FluentResults;
using MediatR;

namespace Rollout.Application.Features.FeatureFlags.Update;

public sealed record UpdateFeatureFlagCommand(
    Guid Id,
    string Name,
    string Description,
    bool IsEnabled,
    int RolloutPercentage) : IRequest<Result>;
