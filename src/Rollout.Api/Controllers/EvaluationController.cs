using Asp.Versioning;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rollout.Application.Features.FeatureFlags.Evaluation;

namespace Rollout.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/evaluate")]
[ApiController]
public sealed class EvaluationController : ControllerBase
{
    private readonly ISender _sender;

    public EvaluationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string key, [FromQuery] string userId)
    {
        var query = new EvaluateFeatureQuery(key, userId);
        Result<EvaluationResponseDto> result = await _sender.Send(query);

        if (result.IsFailed)
        {
            bool isNotFound = result.Errors.Any(error => error.Message == "Feature flag not found.");
            return isNotFound
                ? NotFound(new { errors = result.Errors.Select(error => error.Message) })
                : BadRequest(new { errors = result.Errors.Select(error => error.Message) });
        }

        return Ok(result.Value);
    }
}
