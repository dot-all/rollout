using Rollout.Domain.Entities;

namespace Rollout.Domain.Services;

public interface IFeatureEvaluator
{
    bool Evaluate(FeatureFlag featureFlag, string userId);
}
