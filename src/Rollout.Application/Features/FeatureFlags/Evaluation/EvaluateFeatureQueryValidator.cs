using FluentValidation;

namespace Rollout.Application.Features.FeatureFlags.Evaluation;

/// <summary>
/// Validator for the <see cref="EvaluateFeatureQuery"/>.
/// </summary>
public sealed class EvaluateFeatureQueryValidator : AbstractValidator<EvaluateFeatureQuery>
{
    public EvaluateFeatureQueryValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Key is required.");

        RuleFor(x => x.UserContext)
            .NotNull().WithMessage("User context is required.");

        RuleFor(x => x.UserContext.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
}

