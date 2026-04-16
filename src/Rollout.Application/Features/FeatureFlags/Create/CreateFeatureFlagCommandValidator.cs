using System;
using FluentValidation;

namespace Rollout.Application.Features.FeatureFlags.Create;

/// <summary>
/// Validator for the <see cref="CreateFeatureFlagCommand"/>.
/// Enforces business rules for keys, naming, and rollout constraints.
/// </summary>
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

        RuleForEach(command => command.TargetingRules).ChildRules(rule =>
        {
            rule.RuleFor(r => r.Attribute)
                .NotEmpty().WithMessage("Targeting rule attribute is required.")
                .MaximumLength(100);

            rule.RuleFor(r => r.Operator)
                .NotEmpty().WithMessage("Targeting rule operator is required.")
                .Must(op => new[] { "Equals", "In", "Contains" }.Contains(op, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Operator must be Equals, In, or Contains.");

            rule.RuleFor(r => r.Value)
                .NotEmpty().WithMessage("Targeting rule value is required.")
                .MaximumLength(500);
        });
    }
}

