using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Xunit;

namespace Rollout.IntegrationTests;

[CollectionDefinition("Integration tests")]
public sealed class IntegrationTestCollection : ICollectionFixture<TestcontainersFixture>
{
}

public sealed class TestcontainersFixture : IAsyncLifetime
{
    public PostgreSqlContainer Postgres { get; }
    public RedisContainer Redis { get; }

    public TestcontainersFixture()
    {
        Postgres = new PostgreSqlBuilder("postgres:15-alpine")
            .WithDatabase("rollout_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        Redis = new RedisBuilder("redis:7.0-alpine")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await Postgres.StartAsync();
        await Redis.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Redis.StopAsync();
        await Postgres.StopAsync();
    }
}
