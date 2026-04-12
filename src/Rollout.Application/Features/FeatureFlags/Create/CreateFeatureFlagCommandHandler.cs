using FluentResults;
using MediatR;
using Rollout.Application.Common.Interfaces;
using Rollout.Domain.Entities;

namespace Rollout.Application.Features.FeatureFlags.Create;

public sealed class CreateFeatureFlagCommandHandler : IRequestHandler<CreateFeatureFlagCommand, Result<Guid>>
{
    private readonly IFeatureFlagRepository _featureFlagRepository;

    public CreateFeatureFlagCommandHandler(IFeatureFlagRepository featureFlagRepository)
    {
        _featureFlagRepository = featureFlagRepository;
    }

    public async Task<Result<Guid>> Handle(CreateFeatureFlagCommand request, CancellationToken cancellationToken)
    {
        if (await _featureFlagRepository.ExistsByKeyAsync(request.Key, cancellationToken))
        {
            var duplicateKeyError = new Error("Feature flag key already exists.")
                .WithMetadata("ErrorCode", "DuplicateKey");

            return Result.Fail<Guid>(duplicateKeyError);
        }

        var featureFlag = FeatureFlag.Create(
            request.Key,
            request.Name,
            request.Description,
            request.IsEnabled,
            request.RolloutPercentage);

        await _featureFlagRepository.AddAsync(featureFlag, cancellationToken);

        return Result.Ok(featureFlag.Id);
    }
}
