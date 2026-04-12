using FluentResults;
using MediatR;

namespace Rollout.Application.Features.FeatureFlags.Create;

public sealed record CreateFeatureFlagCommand(
    string Key,
    string Name,
    string Description,
    bool IsEnabled,
    int RolloutPercentage) : IRequest<Result<Guid>>;
