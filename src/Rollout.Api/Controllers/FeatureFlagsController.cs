using Asp.Versioning;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rollout.Application.Features.FeatureFlags;
using Rollout.Application.Features.FeatureFlags.Create;
using Rollout.Application.Features.FeatureFlags.Delete;
using Rollout.Application.Features.FeatureFlags.Read;
using Rollout.Application.Features.FeatureFlags.Update;

namespace Rollout.Api.Controllers;

[ApiVersion(1)]
[Route("api/featureflags")]
[ApiController]
public sealed class FeatureFlagsController : ControllerBase
{
    private readonly ISender _sender;

    public FeatureFlagsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        Result<FeatureFlagDto> result = await _sender.Send(new GetFeatureFlagByIdQuery(id));

        if (result.IsFailed)
        {
            return NotFound(new { errors = result.Errors.Select(error => error.Message) });
        }

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        Result<IEnumerable<FeatureFlagDto>> result = await _sender.Send(new GetAllFeatureFlagsQuery());

        if (result.IsFailed)
        {
            return BadRequest(new { errors = result.Errors.Select(error => error.Message) });
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFeatureFlagRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest(new { errors = new[] { "Route id and payload id must match." } });
        }

        var command = new UpdateFeatureFlagCommand(
            request.Id,
            request.Name,
            request.Description,
            request.IsEnabled,
            request.RolloutPercentage);

        Result result = await _sender.Send(command);

        if (result.IsFailed)
        {
            bool isNotFound = result.Errors.Any(error => error.Message == "Feature flag not found.");
            return isNotFound
                ? NotFound(new { errors = result.Errors.Select(error => error.Message) })
                : BadRequest(new { errors = result.Errors.Select(error => error.Message) });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        Result result = await _sender.Send(new DeleteFeatureFlagCommand(id));

        if (result.IsFailed)
        {
            return NotFound(new { errors = result.Errors.Select(error => error.Message) });
        }

        return NoContent();
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

        return Created($"/api/featureflags/{result.Value}", new { id = result.Value });
    }

    public sealed record CreateFeatureFlagRequest(
        string Key,
        string Name,
        string Description,
        bool IsEnabled,
        int RolloutPercentage);

    public sealed record UpdateFeatureFlagRequest(
        Guid Id,
        string Name,
        string Description,
        bool IsEnabled,
        int RolloutPercentage);
}
