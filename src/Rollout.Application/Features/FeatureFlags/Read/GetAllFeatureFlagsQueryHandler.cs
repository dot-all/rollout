using System.Linq;
using FluentResults;
using MediatR;
using Rollout.Application.Common.Interfaces;

namespace Rollout.Application.Features.FeatureFlags.Read;

public sealed class GetAllFeatureFlagsQueryHandler : IRequestHandler<GetAllFeatureFlagsQuery, Result<IEnumerable<FeatureFlagDto>>>
{
    private readonly IFeatureFlagRepository _featureFlagRepository;

    public GetAllFeatureFlagsQueryHandler(IFeatureFlagRepository featureFlagRepository)
    {
        _featureFlagRepository = featureFlagRepository;
    }

    public async Task<Result<IEnumerable<FeatureFlagDto>>> Handle(GetAllFeatureFlagsQuery request, CancellationToken cancellationToken)
    {
        var featureFlags = await _featureFlagRepository.GetAllAsync(cancellationToken);
        var result = featureFlags.Select(FeatureFlagDto.FromEntity);

        return Result.Ok(result);
    }
}
