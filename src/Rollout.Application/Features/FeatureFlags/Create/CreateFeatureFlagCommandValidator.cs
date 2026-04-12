using FluentValidation;

namespace Rollout.Application.Features.FeatureFlags.Create;

public sealed class CreateFeatureFlagCommandValidator : AbstractValidator<CreateFeatureFlagCommand>
{
    public CreateFeatureFlagCommandValidator()
    {
        RuleFor(command => command.Key)
            .NotEmpty().WithMessage("Key is required.")
            .Matches(@"^[a-z0-9\-]+$").WithMessage("Key must contain only lowercase letters, numbers, and hyphens.")
            .MaximumLength(100);

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
