using Rollout.Domain;

namespace Rollout.Domain.Entities;

/// <summary>
/// Represents a feature flag entity that governs whether a specific feature is active for a given user context.
/// This entity follows a rich domain model pattern, ensuring internal consistency through private setters and validation.
/// </summary>
public sealed class FeatureFlag
{
    public Guid Id { get; private set; }
    public string Key { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool IsEnabled { get; private set; }
    public int RolloutPercentage { get; private set; }
    public List<TargetingRule> TargetingRules { get; private set; } = new();

    private FeatureFlag() { }

    private FeatureFlag(
        string key,
        string name,
        string description,
        bool isEnabled,
        int rolloutPercentage,
        IEnumerable<TargetingRule>? targetingRules)
    {
        ValidateState(key, name, description, rolloutPercentage);

        Id = Guid.NewGuid();
        Key = key;
        Name = name;
        Description = description;
        IsEnabled = isEnabled;
        RolloutPercentage = rolloutPercentage;
        SetTargetingRules(targetingRules);
    }

    /// <summary>
    /// Factory method to create a new instance of a FeatureFlag.
    /// </summary>
    public static FeatureFlag Create(
        string key,
        string name,
        string description,
        bool isEnabled,
        int rolloutPercentage,
        IEnumerable<TargetingRule>? targetingRules = null)
        => new FeatureFlag(key, name, description, isEnabled, rolloutPercentage, targetingRules);

    /// <summary>
    /// Toggles the overall enabled state of the feature flag.
    /// </summary>
    public void SetEnabled(bool isEnabled) => IsEnabled = isEnabled;

    /// <summary>
    /// Updates the rollout percentage (0-100) for the feature flag.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when percentage is outside the valid range.</exception>
    public void UpdateRolloutPercentage(int rolloutPercentage)
    {
        if (rolloutPercentage < 0 || rolloutPercentage > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(rolloutPercentage), "Rollout percentage must be between 0 and 100.");
        }

        RolloutPercentage = rolloutPercentage;
    }

    /// <summary>
    /// Updates the display name of the feature flag.
    /// </summary>
    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        Name = name;
    }

    /// <summary>
    /// Updates the description of the feature flag.
    /// </summary>
    public void UpdateDescription(string description)
    {
        if (description is null)
        {
            throw new ArgumentNullException(nameof(description));
        }

        Description = description;
    }

    /// <summary>
    /// Updates the targeting rules that refine which users are eligible for this feature.
    /// </summary>
    public void UpdateTargetingRules(IEnumerable<TargetingRule>? targetingRules)
    {
        // Null means no change or clear depending on interpretation; here we treat null/empty as removing all rules.
        SetTargetingRules(targetingRules);
    }

    /// <summary>
    /// Checks if the provided user context matches the defined targeting rules.
    /// If no rules are defined, it matches by default.
    /// </summary>
    public bool MatchesRules(UserContext context)
    {
        if (context is null)
        {
            return false;
        }

        return TargetingRules.Count == 0 || TargetingRules.All(rule => rule.Matches(context));
    }

    private void SetTargetingRules(IEnumerable<TargetingRule>? targetingRules)
    {
        TargetingRules = targetingRules?.ToList() ?? new List<TargetingRule>();
    }

    private static void ValidateState(string key, string name, string description, int rolloutPercentage)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Feature flag key is required.", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Feature flag name is required.", nameof(name));
        }

        if (description is null)
        {
            throw new ArgumentNullException(nameof(description));
        }

        if (rolloutPercentage < 0 || rolloutPercentage > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(rolloutPercentage), "Rollout percentage must be between 0 and 100.");
        }
    }
}

