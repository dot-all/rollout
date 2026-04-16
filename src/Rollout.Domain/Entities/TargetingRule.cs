namespace Rollout.Domain.Entities;

/// <summary>
/// Defines a rule used to target specific users based on their attributes.
/// Supporting operators include equality, inclusion in set, and substring containment.
/// </summary>
public sealed class TargetingRule
{
    public string Attribute { get; private set; } = null!;
    public string Operator { get; private set; } = null!;
    public string Value { get; private set; } = null!;

    private TargetingRule() { }

    public TargetingRule(string attribute, string @operator, string value)
    {
        if (string.IsNullOrWhiteSpace(attribute))
        {
            throw new ArgumentException("Attribute is required.", nameof(attribute));
        }

        if (string.IsNullOrWhiteSpace(@operator))
        {
            throw new ArgumentException("Operator is required.", nameof(@operator));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", nameof(value));
        }

        Attribute = attribute;
        Operator = @operator;
        Value = value;
    }

    /// <summary>
    /// Evaluates whether the user context satisfies this specific rule.
    /// Case-insensitivity is prioritized for robust matching across different clients.
    /// </summary>
    public bool Matches(UserContext context)
    {
        if (context is null)
        {
            return false;
        }

        if (!context.Attributes.TryGetValue(Attribute, out var actualValue))
        {
            return false;
        }

        return Operator.ToLowerInvariant() switch
        {
            "equals" => string.Equals(actualValue, Value, StringComparison.OrdinalIgnoreCase),
            "in" => Value.Split(',')
                .Select(item => item.Trim())
                .Any(candidate => string.Equals(actualValue, candidate, StringComparison.OrdinalIgnoreCase)),
            "contains" => actualValue.Contains(Value, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }
}

