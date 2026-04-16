using FluentResults;
using MediatR;
using Rollout.Application.Common.Interfaces;
using Rollout.Application.Features.FeatureFlags.Notifications;

namespace Rollout.Application.Features.FeatureFlags.Delete;

/// <summary>
/// Handles the deletion of a feature flag.
/// Ensures the flag is removed from persistence and broadcasts the deletion event.
/// </summary>
public sealed class DeleteFeatureFlagCommandHandler : IRequestHandler<DeleteFeatureFlagCommand, Result>
{
    private readonly IFeatureFlagRepository _featureFlagRepository;
    private readonly IPublisher _publisher;

    public DeleteFeatureFlagCommandHandler(IFeatureFlagRepository featureFlagRepository, IPublisher publisher)
    {
        _featureFlagRepository = featureFlagRepository;
        _publisher = publisher;
    }

    public async Task<Result> Handle(DeleteFeatureFlagCommand request, CancellationToken cancellationToken)
    {
        var featureFlag = await _featureFlagRepository.GetByIdAsync(request.Id, cancellationToken);

        if (featureFlag is null)
        {
            return Result.Fail(new Error("Feature flag not found."));
        }

        await _featureFlagRepository.DeleteAsync(featureFlag, cancellationToken);
        
        // Notify subscribers that the flag is no longer available. 
        // This is important for cleanup in consumer services.
        await _publisher.Publish(new FeatureFlagChangedNotification(featureFlag.Key, "flag_deleted"), cancellationToken);

        return Result.Ok();
    }
}

