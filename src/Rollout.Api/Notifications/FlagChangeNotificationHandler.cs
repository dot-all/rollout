using MediatR;
using Rollout.Application.Features.FeatureFlags.Notifications;
using Rollout.Api.Services;

namespace Rollout.Api.Notifications;

/// <summary>
/// Reacts to <see cref="FeatureFlagChangedNotification"/> domain events and forwards them to the API notification service.
/// This acts as a bridge between the Application layer events and the Web/API specialized notification mechanisms (like SSE).
/// </summary>
public sealed class FlagChangeNotificationHandler : INotificationHandler<FeatureFlagChangedNotification>
{
    private readonly NotificationService _notificationService;

    public FlagChangeNotificationHandler(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task Handle(FeatureFlagChangedNotification notification, CancellationToken cancellationToken)
    {
        _notificationService.Publish(new NotificationMessage(notification.Key, notification.EventType));
        return Task.CompletedTask;
    }
}

