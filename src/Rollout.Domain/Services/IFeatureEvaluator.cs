using Rollout.Domain.Entities;
using Rollout.Domain;

namespace Rollout.Domain.Services;

/// <summary>
/// Defines the core logic for evaluating whether a feature flag is enabled for a specific user.
/// </summary>
public interface IFeatureEvaluator
{
    /// <summary>
    /// Evaluates the feature flag state based on its configuration and the user context.
    /// Evaluation includes checking the global toggle, targeting rules, and deterministic rollout percentage.
    /// </summary>
    bool Evaluate(FeatureFlag featureFlag, UserContext userContext);
}

