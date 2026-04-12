using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rollout.Application.Features.FeatureFlags.Create;

namespace Rollout.Api.Controllers;

[ApiController]
[Route("api/feature-flags")]
public sealed class FeatureFlagsController : ControllerBase
{
    private readonly ISender _sender;

    public FeatureFlagsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFeatureFlagRequest request)
    {
        var command = new CreateFeatureFlagCommand(
            request.Key,
            request.Name,
            request.Description,
            request.IsEnabled,
            request.RolloutPercentage);

        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailed)
        {
            bool isConflict = result.Errors.Any(error =>
                error.Metadata.TryGetValue("ErrorCode", out var value) && value?.ToString() == "DuplicateKey");

            var response = new { errors = result.Errors.Select(error => error.Message) };
            return isConflict ? Conflict(response) : BadRequest(response);
        }

        return Created($"/api/feature-flags/{result.Value}", new { id = result.Value });
    }

    public sealed record CreateFeatureFlagRequest(
        string Key,
        string Name,
        string Description,
        bool IsEnabled,
        int RolloutPercentage);
}
