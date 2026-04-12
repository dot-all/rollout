using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using Rollout.Domain.Entities;

namespace Rollout.Domain.Services;

public sealed class FeatureEvaluator : IFeatureEvaluator
{
    public bool Evaluate(FeatureFlag featureFlag, string userId)
    {
        if (!featureFlag.IsEnabled)
        {
            return false;
        }

        if (featureFlag.RolloutPercentage == 100)
        {
            return true;
        }

        if (featureFlag.RolloutPercentage == 0)
        {
            return false;
        }

        string payload = $"{featureFlag.Key}-{userId}";
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(payload));

        // Use a fixed 32-bit sample from the hash digest and map into 1..100.
        // This preserves deterministic bucketing across the same key/user pair.
        uint bucket = BinaryPrimitives.ReadUInt32LittleEndian(hash);
        int mappedValue = (int)(bucket % 100) + 1;

        return mappedValue <= featureFlag.RolloutPercentage;
    }
}
