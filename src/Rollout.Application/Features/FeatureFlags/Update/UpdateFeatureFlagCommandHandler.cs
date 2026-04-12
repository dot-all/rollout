using FluentResults;
using MediatR;
using Rollout.Application.Common.Interfaces;

namespace Rollout.Application.Features.FeatureFlags.Update;

public sealed class UpdateFeatureFlagCommandHandler : IRequestHandler<UpdateFeatureFlagCommand, Result>
{
    private readonly IFeatureFlagRepository _featureFlagRepository;

    public UpdateFeatureFlagCommandHandler(IFeatureFlagRepository featureFlagRepository)
    {
        _featureFlagRepository = featureFlagRepository;
    }

    public async Task<Result> Handle(UpdateFeatureFlagCommand request, CancellationToken cancellationToken)
    {
        var featureFlag = await _featureFlagRepository.GetByIdAsync(request.Id, cancellationToken);

        if (featureFlag is null)
        {
            return Result.Fail(new Error("Feature flag not found."));
        }

        featureFlag.Rename(request.Name);
        featureFlag.UpdateDescription(request.Description);
        featureFlag.SetEnabled(request.IsEnabled);
        featureFlag.UpdateRolloutPercentage(request.RolloutPercentage);

        await _featureFlagRepository.UpdateAsync(featureFlag, cancellationToken);

        return Result.Ok();
    }
}
