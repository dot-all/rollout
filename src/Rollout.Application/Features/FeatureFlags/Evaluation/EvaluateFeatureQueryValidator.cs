using FluentValidation;

namespace Rollout.Application.Features.FeatureFlags.Evaluation;

public sealed class EvaluateFeatureQueryValidator : AbstractValidator<EvaluateFeatureQuery>
{
    public EvaluateFeatureQueryValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Key is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
}
