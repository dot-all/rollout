namespace Rollout.Domain.Entities;

public sealed class FeatureFlag
{
    public Guid Id { get; private set; }
    public string Key { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool IsEnabled { get; private set; }
    public int RolloutPercentage { get; private set; }

    private FeatureFlag() { }

    private FeatureFlag(string key, string name, string description, bool isEnabled, int rolloutPercentage)
    {
        ValidateState(key, name, description, rolloutPercentage);

        Id = Guid.NewGuid();
        Key = key;
        Name = name;
        Description = description;
        IsEnabled = isEnabled;
        RolloutPercentage = rolloutPercentage;
    }

    public static FeatureFlag Create(string key, string name, string description, bool isEnabled, int rolloutPercentage)
        => new FeatureFlag(key, name, description, isEnabled, rolloutPercentage);

    public void SetEnabled(bool isEnabled) => IsEnabled = isEnabled;

    public void UpdateRolloutPercentage(int rolloutPercentage)
    {
        if (rolloutPercentage < 0 || rolloutPercentage > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(rolloutPercentage), "Rollout percentage must be between 0 and 100.");
        }

        RolloutPercentage = rolloutPercentage;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        Name = name;
    }

    public void UpdateDescription(string description)
    {
        if (description is null)
        {
            throw new ArgumentNullException(nameof(description));
        }

        Description = description;
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
