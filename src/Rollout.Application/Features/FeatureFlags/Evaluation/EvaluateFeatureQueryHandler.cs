using FluentResults;
using MediatR;
using Rollout.Application.Common.Interfaces;
using Rollout.Domain;
using Rollout.Domain.Services;

namespace Rollout.Application.Features.FeatureFlags.Evaluation;

/// <summary>
/// Handles the evaluation of a feature flag.
/// Bridging the persistence layer and the domain's evaluation logic.
/// </summary>
public sealed class EvaluateFeatureQueryHandler : IRequestHandler<EvaluateFeatureQuery, Result<EvaluationResponseDto>>
{
    private readonly IFeatureFlagRepository _featureFlagRepository;
    private readonly IFeatureEvaluator _featureEvaluator;

    public EvaluateFeatureQueryHandler(
        IFeatureFlagRepository featureFlagRepository,
        IFeatureEvaluator featureEvaluator)
    {
        _featureFlagRepository = featureFlagRepository;
        _featureEvaluator = featureEvaluator;
    }

    public async Task<Result<EvaluationResponseDto>> Handle(EvaluateFeatureQuery request, CancellationToken cancellationToken)
    {
        var featureFlag = await _featureFlagRepository.GetByKeyAsync(request.Key, cancellationToken);

        if (featureFlag is null)
        {
            return Result.Fail<EvaluationResponseDto>(new Error("Feature flag not found."));
        }

        // Map the Application DTO context to the Domain model context.
        var userContext = new UserContext(
            request.UserContext.UserId,
            request.UserContext.Attributes ?? new Dictionary<string, string>());

        // Delegate the actual algorithmic decision to the domain service.
        bool isEnabledForUser = _featureEvaluator.Evaluate(featureFlag, userContext);
        
        var response = new EvaluationResponseDto(featureFlag.Key, isEnabledForUser);

        return Result.Ok(response);
    }
}

