using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using StackExchange.Redis;
using Xunit;

namespace Rollout.IntegrationTests;

[Collection("Integration tests")]
public sealed class FeatureFlagIntegrationTests
{
    private readonly HttpClient _client;
    private readonly IConnectionMultiplexer _redis;

    public FeatureFlagIntegrationTests(TestcontainersFixture fixture)
    {
        var factory = new CustomWebApplicationFactory(fixture.Postgres, fixture.Redis);
        _client = factory.CreateClient();
        _redis = ConnectionMultiplexer.Connect(fixture.Redis.GetConnectionString());
    }

    [Fact]
    public async Task CreateEvaluateAndInvalidateCacheFlow()
    {
        const string key = "enterprise-feature";

        var createPayload = new
        {
            Key = key,
            Name = "Enterprise Feature",
            Description = "Feature with targeting rules.",
            IsEnabled = true,
            RolloutPercentage = 100,
            TargetingRules = new[]
            {
                new { Attribute = "country", Operator = "Equals", Value = "CL" }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/featureflags", createPayload);
        createResponse.EnsureSuccessStatusCode();

        var createContent = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = createContent.GetProperty("id").GetGuid();
        id.Should().NotBe(Guid.Empty);

        var userContext = new
        {
            UserId = "user-123",
            Attributes = new Dictionary<string, string>
            {
                ["country"] = "CL"
            }
        };

        var evaluateResponse = await _client.PostAsJsonAsync($"/api/v1/evaluate/{key}", userContext);
        evaluateResponse.EnsureSuccessStatusCode();

        var evaluation = await evaluateResponse.Content.ReadFromJsonAsync<JsonElement>();
        evaluation.GetProperty("isEnabledForUser").GetBoolean().Should().BeTrue();

        var redisDb = _redis.GetDatabase();
        var cacheKey = $"featureflag:{key}";
        
        // IDistributedCache in ASP.NET Core with Redis implementation stores data in a Hash under the 'data' field.
        var cachedValue = await redisDb.HashGetAsync(cacheKey, "data");
        cachedValue.HasValue.Should().BeTrue();


        var updatePayload = new
        {
            Id = id,
            Name = "Enterprise Feature Updated",
            Description = "Updated feature.",
            IsEnabled = false,
            RolloutPercentage = 0,
            TargetingRules = new object[0]
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/featureflags/{id}", updatePayload);
        updateResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        var cachedAfterUpdate = await redisDb.KeyExistsAsync(cacheKey);
        cachedAfterUpdate.Should().BeFalse();

    }
}
