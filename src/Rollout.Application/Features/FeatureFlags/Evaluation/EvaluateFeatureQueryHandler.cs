using FluentResults;
using MediatR;
using Rollout.Application.Common.Interfaces;
using Rollout.Domain.Services;

namespace Rollout.Application.Features.FeatureFlags.Evaluation;

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

        bool isEnabledForUser = _featureEvaluator.Evaluate(featureFlag, request.UserId);
        var response = new EvaluationResponseDto(featureFlag.Key, isEnabledForUser);

        return Result.Ok(response);
    }
}
