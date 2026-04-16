using FluentResults;
using MediatR;
using Rollout.Application.Common.Interfaces;
using Rollout.Application.Features.FeatureFlags.Notifications;
using Rollout.Domain.Entities;

namespace Rollout.Application.Features.FeatureFlags.Update;

/// <summary>
/// Handles updates to an existing feature flag configuration.
/// Ensures the flag exists before applying changes and notifies subscribers of the modification.
/// </summary>
public sealed class UpdateFeatureFlagCommandHandler : IRequestHandler<UpdateFeatureFlagCommand, Result>
{
    private readonly IFeatureFlagRepository _featureFlagRepository;
    private readonly IPublisher _publisher;

    public UpdateFeatureFlagCommandHandler(IFeatureFlagRepository featureFlagRepository, IPublisher publisher)
    {
        _featureFlagRepository = featureFlagRepository;
        _publisher = publisher;
    }

    public async Task<Result> Handle(UpdateFeatureFlagCommand request, CancellationToken cancellationToken)
    {
        var featureFlag = await _featureFlagRepository.GetByIdAsync(request.Id, cancellationToken);

        if (featureFlag is null)
        {
            return Result.Fail(new Error("Feature flag not found."));
        }

        // Apply changes through the aggregate's domain methods to ensure business invariants are maintained.
        featureFlag.Rename(request.Name);
        featureFlag.UpdateDescription(request.Description);
        featureFlag.SetEnabled(request.IsEnabled);
        featureFlag.UpdateRolloutPercentage(request.RolloutPercentage);

        if (request.TargetingRules is not null)
        {
            featureFlag.UpdateTargetingRules(request.TargetingRules.Select(rule => new TargetingRule(rule.Attribute, rule.Operator, rule.Value)));
        }

        await _featureFlagRepository.UpdateAsync(featureFlag, cancellationToken);
        
        // Broadcast the update event. This is crucial for distributed environments where instances
        // might need to invalidate their local caches.
        await _publisher.Publish(new FeatureFlagChangedNotification(featureFlag.Key, "flag_updated"), cancellationToken);

        return Result.Ok();
    }
}

