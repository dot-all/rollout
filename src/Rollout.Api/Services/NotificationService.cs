using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Rollout.Api.Services;

/// <summary>
/// Managed service for distributing feature flag change notifications to connected clients.
/// Uses <see cref="Channel{T}"/> for thread-safe, non-blocking asynchronous streaming.
/// </summary>
public sealed class NotificationService
{
    private readonly object _lock = new();
    private readonly List<Channel<NotificationMessage>> _subscribers = new();

    /// <summary>
    /// Publishes a message to all active subscribers.
    /// Fans out the message to each subscriber's individual channel.
    /// </summary>
    public void Publish(NotificationMessage message)
    {
        lock (_lock)
        {
            // Create a temporary copy to avoid modification during iteration.
            foreach (Channel<NotificationMessage> channel in _subscribers.ToArray())
            {
                // TryWrite is used to prevent blocking the publisher if a subscriber's channel is full.
                channel.Writer.TryWrite(message);
            }
        }
    }

    /// <summary>
    /// Subscribes to the notification stream.
    /// Returns an async enumerable that will yield notifications until the cancellation token is triggered.
    /// </summary>
    public IAsyncEnumerable<NotificationMessage> Subscribe(CancellationToken cancellationToken)
    {
        // Unbounded channel for simplicity in this implementation; 
        // in high-load scenarios, a bounded channel with a BoundedChannelFullMode might be safer.
        var channel = Channel.CreateUnbounded<NotificationMessage>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        lock (_lock)
        {
            _subscribers.Add(channel);
        }

        return ReadAllAsync(channel, cancellationToken);
    }

    private async IAsyncEnumerable<NotificationMessage> ReadAllAsync(Channel<NotificationMessage> channel, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Ensure the channel writer is completed when the client disconnects or the request is cancelled.
        using var registration = cancellationToken.Register(() => channel.Writer.TryComplete());

        try
        {
            while (await channel.Reader.WaitToReadAsync(cancellationToken))
            {
                while (channel.Reader.TryRead(out var message))
                {
                    yield return message;
                }
            }
        }
        finally
        {
            // Clean up the subscriber list when the stream ends.
            lock (_lock)
            {
                _subscribers.Remove(channel);
            }
        }
    }
}

/// <summary>
/// Data contract for notifications pushed to clients.
/// </summary>
/// <param name="Key">The business key of the feature flag.</param>
/// <param name="EventType">The descriptive event type.</param>
public sealed record NotificationMessage(string Key, string EventType);

