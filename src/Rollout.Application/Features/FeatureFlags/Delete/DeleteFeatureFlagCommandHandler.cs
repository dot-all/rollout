using FluentResults;
using MediatR;
using Rollout.Application.Common.Interfaces;

namespace Rollout.Application.Features.FeatureFlags.Delete;

public sealed class DeleteFeatureFlagCommandHandler : IRequestHandler<DeleteFeatureFlagCommand, Result>
{
    private readonly IFeatureFlagRepository _featureFlagRepository;

    public DeleteFeatureFlagCommandHandler(IFeatureFlagRepository featureFlagRepository)
    {
        _featureFlagRepository = featureFlagRepository;
    }

    public async Task<Result> Handle(DeleteFeatureFlagCommand request, CancellationToken cancellationToken)
    {
        var featureFlag = await _featureFlagRepository.GetByIdAsync(request.Id, cancellationToken);

        if (featureFlag is null)
        {
            return Result.Fail(new Error("Feature flag not found."));
        }

        await _featureFlagRepository.DeleteAsync(featureFlag, cancellationToken);

        return Result.Ok();
    }
}
