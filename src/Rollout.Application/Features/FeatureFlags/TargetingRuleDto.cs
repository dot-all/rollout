namespace Rollout.Application.Features.FeatureFlags;

/// <summary>
/// Data transfer object for targeting rules used in commands and queries.
/// </summary>
/// <param name="Attribute">The user attribute key (e.g., 'Email', 'Role').</param>
/// <param name="Operator">The matching operation (e.g., 'Equals', 'In', 'Contains').</param>
/// <param name="Value">The value(s) to match against. For 'In' operator, use comma-separated values.</param>
public sealed record TargetingRuleDto(string Attribute, string Operator, string Value)
{
    /// <summary>
    /// Maps a domain entity to its DTO representation.
    /// </summary>
    public static TargetingRuleDto FromDomain(Domain.Entities.TargetingRule rule)
        => new(rule.Attribute, rule.Operator, rule.Value);
}

