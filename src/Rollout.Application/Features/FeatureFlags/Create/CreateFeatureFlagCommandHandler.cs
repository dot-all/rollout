using FluentResults;
using MediatR;
using Rollout.Application.Common.Interfaces;
using Rollout.Application.Features.FeatureFlags.Notifications;
using Rollout.Domain.Entities;

namespace Rollout.Application.Features.FeatureFlags.Create;

/// <summary>
/// Handles the creation of a new feature flag. 
/// Orchestrates validation, persistence, and external notifications.
/// </summary>
public sealed class CreateFeatureFlagCommandHandler : IRequestHandler<CreateFeatureFlagCommand, Result<Guid>>
{
    private readonly IFeatureFlagRepository _featureFlagRepository;
    private readonly IPublisher _publisher;

    public CreateFeatureFlagCommandHandler(IFeatureFlagRepository featureFlagRepository, IPublisher publisher)
    {
        _featureFlagRepository = featureFlagRepository;
        _publisher = publisher;
    }

    public async Task<Result<Guid>> Handle(CreateFeatureFlagCommand request, CancellationToken cancellationToken)
    {
        // Check for uniqueness. The business key must be unique across the entire system.
        if (await _featureFlagRepository.ExistsByKeyAsync(request.Key, cancellationToken))
        {
            var duplicateKeyError = new Error("Feature flag key already exists.")
                .WithMetadata("ErrorCode", "DuplicateKey");

            return Result.Fail<Guid>(duplicateKeyError);
        }

        // Map DTOs to Domain Entities through the aggregate root factory method.
        var featureFlag = FeatureFlag.Create(
            request.Key,
            request.Name,
            request.Description,
            request.IsEnabled,
            request.RolloutPercentage,
            request.TargetingRules?.Select(rule => new TargetingRule(rule.Attribute, rule.Operator, rule.Value)));

        await _featureFlagRepository.AddAsync(featureFlag, cancellationToken);
        
        // Notify other parts of the system (or external subscribers) about the new flag configuration.
        await _publisher.Publish(new FeatureFlagChangedNotification(featureFlag.Key, "flag_created"), cancellationToken);

        return Result.Ok(featureFlag.Id);
    }
}

