using System.Text.Json;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Rollout.Api.Services;

namespace Rollout.Api.Controllers;

/// <summary>
/// Provides real-time notifications about feature flag changes.
/// </summary>
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/flags")]
[ApiController]
public sealed class FlagsController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public FlagsController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Establishes a Server-Sent Events (SSE) stream to push flag change notifications to clients.
    /// This allows clients to invalidate their local caches in real-time without polling.
    /// </summary>
    [HttpGet("stream")]
    public async Task Stream(CancellationToken cancellationToken)
    {
        // Set the appropriate content type for SSE as per RFC 8895.
        Response.ContentType = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";

        await foreach (var message in _notificationService.Subscribe(cancellationToken))
        {
            var payload = JsonSerializer.Serialize(message);
            
            // Format the message according to the SSE protocol.
            await Response.WriteAsync($"event: {message.EventType}\n");
            await Response.WriteAsync($"data: {payload}\n\n");
            
            // Flush the body immediately to ensure the client receives the event without buffering delays.
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}

