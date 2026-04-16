namespace Rollout.Domain;

/// <summary>
/// Encapsulates the runtime context for a specific user, including their unique identifier 
/// and any arbitrary attributes used for feature flag targeting logic.
/// </summary>
public sealed record UserContext
{
    public string UserId { get; init; }
    public IReadOnlyDictionary<string, string> Attributes { get; init; }

    public UserContext(string userId, IDictionary<string, string>? attributes = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("UserId is required.", nameof(userId));
        }

        UserId = userId;
        // Attributes are case-insensitive to ensure reliable matching regardless of how keys are passed from clients.
        Attributes = new Dictionary<string, string>(attributes ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
    }
}

