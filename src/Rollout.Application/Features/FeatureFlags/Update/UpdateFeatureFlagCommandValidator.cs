using System;
using FluentValidation;

namespace Rollout.Application.Features.FeatureFlags.Update;

/// <summary>
/// Validator for the <see cref="UpdateFeatureFlagCommand"/>.
/// </summary>
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

