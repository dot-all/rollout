using FluentValidation;

namespace Rollout.Application.Features.FeatureFlags.Update;

public sealed class UpdateFeatureFlagCommandValidator : AbstractValidator<UpdateFeatureFlagCommand>
{
    public UpdateFeatureFlagCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .NotNull().WithMessage("Description is required.")
            .MaximumLength(1000);

        RuleFor(command => command.RolloutPercentage)
            .InclusiveBetween(0, 100).WithMessage("Rollout percentage must be between 0 and 100.");
    }
}
