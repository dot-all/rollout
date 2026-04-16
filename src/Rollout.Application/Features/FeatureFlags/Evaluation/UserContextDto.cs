namespace Rollout.Application.Features.FeatureFlags.Evaluation;

/// <summary>
/// Data transfer object for user context information during evaluation.
/// </summary>
/// <param name="UserId">The unique identifier of the user (e.g., GUID or email).</param>
/// <param name="Attributes">Arbitrary key-value pairs representing user traits (e.g., 'country', 'plan').</param>
public sealed record UserContextDto(string UserId, Dictionary<string, string>? Attributes)
{
    /// <summary>
    /// Creates an empty context for a user with no additional attributes.
    /// </summary>
    public static UserContextDto Empty(string userId) => new(userId, new Dictionary<string, string>());
}

