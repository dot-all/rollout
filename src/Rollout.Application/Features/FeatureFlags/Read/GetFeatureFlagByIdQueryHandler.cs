using FluentResults;
using MediatR;
using Rollout.Application.Common.Interfaces;

namespace Rollout.Application.Features.FeatureFlags.Read;

public sealed class GetFeatureFlagByIdQueryHandler : IRequestHandler<GetFeatureFlagByIdQuery, Result<FeatureFlagDto>>
{
    private readonly IFeatureFlagRepository _featureFlagRepository;

    public GetFeatureFlagByIdQueryHandler(IFeatureFlagRepository featureFlagRepository)
    {
        _featureFlagRepository = featureFlagRepository;
    }

    public async Task<Result<FeatureFlagDto>> Handle(GetFeatureFlagByIdQuery request, CancellationToken cancellationToken)
    {
        var featureFlag = await _featureFlagRepository.GetByIdAsync(request.Id, cancellationToken);

        if (featureFlag is null)
        {
            return Result.Fail<FeatureFlagDto>(new Error("Feature flag not found."));
        }

        return Result.Ok(FeatureFlagDto.FromEntity(featureFlag));
    }
}
