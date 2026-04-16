using MediatR;

namespace Rollout.Application.Features.FeatureFlags.Notifications;

/// <summary>
/// Domain event published whenever a feature flag is created, updated, or deleted. 
/// Used for cross-component orchestration and external caching invalidation.
/// </summary>
/// <param name="Key">The business key of the affected feature flag.</param>
/// <param name="EventType">The type of event (e.g., 'flag_created', 'flag_updated', 'flag_deleted').</param>
public sealed record FeatureFlagChangedNotification(string Key, string EventType) : INotification;

