using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using Rollout.Domain;
using Rollout.Domain.Entities;

namespace Rollout.Domain.Services;

/// <summary>
/// Implements the default feature evaluation logic emphasizing determinism and performance.
/// </summary>
public sealed class FeatureEvaluator : IFeatureEvaluator
{
    /// <summary>
    /// Evaluates the feature toggle status. The process follows a short-circuiting sequence:
    /// 1. Global kill-switch status.
    /// 2. Targeting rule matching.
    /// 3. Deterministic bucketing for gradual rollouts.
    /// </summary>
    public bool Evaluate(FeatureFlag featureFlag, UserContext userContext)
    {
        // Short-circuit if the feature is explicitly disabled globally.
        if (!featureFlag.IsEnabled)
        {
            return false;
        }

        // Evaluate targeting rules; if the user doesn't fit the specified criteria, they are excluded.
        if (!featureFlag.MatchesRules(userContext))
        {
            return false;
        }

        // Quick return for edge rollout percentages.
        if (featureFlag.RolloutPercentage == 100)
        {
            return true;
        }

        if (featureFlag.RolloutPercentage == 0)
        {
            return false;
        }

        // To ensure a consistent experience, users must be deterministically bucketed.
        // We use a hash of (FeatureKey + UserId) so a user always falls into the same bucket for the same flag.
        string payload = $"{featureFlag.Key}-{userContext.UserId}";
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(payload));

        // Use a fixed 32-bit sample from the hash digest and map into a 1..100 range.
        // LittleEndian reading ensures consistency across different hardware architectures.
        uint bucketValue = BinaryPrimitives.ReadUInt32LittleEndian(hash);
        int mappedBucket = (int)(bucketValue % 100) + 1;

        return mappedBucket <= featureFlag.RolloutPercentage;
    }
}

