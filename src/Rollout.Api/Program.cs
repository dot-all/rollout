using Asp.Versioning;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Rollout.Application.Common.Interfaces;
using Rollout.Application.Common.Pipeline;
using Rollout.Application.Features.FeatureFlags.Create;
using Rollout.Api.Services;
using Rollout.Domain.Services;
using Rollout.Infrastructure.Persistence;
using Rollout.Infrastructure.Persistence.Repositories;
using Scalar.AspNetCore;

// Use the specialized builders to configure the WebHost and services.
var builder = WebApplication.CreateBuilder(args);

// --- Service Registration ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Configure API versioning to support backward compatibility and clean URL versioning.
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

// Configure MediatR with the application assembly and register the cross-cutting validation pipeline.
builder.Services.AddMediatR(typeof(CreateFeatureFlagCommand).Assembly, typeof(NotificationService).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateFeatureFlagCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Register domain and infrastructure abstractions.
builder.Services.AddScoped<IFeatureFlagRepository, FeatureFlagRepository>();
builder.Services.AddScoped<IFeatureEvaluator, FeatureEvaluator>();

// Notification service is a Singleton as it manages ephemeral in-memory subscriptions (SSE).
builder.Services.AddSingleton<NotificationService>();

// Configure out-of-process caching (Redis) for high availability and consistency across instances.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});

builder.Services.AddDbContext<RolloutDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Application Pipeline ---

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); 
    
    // Scalar provides an enhanced UI for OpenAPI exploration, using a modern 'DeepSpace' theme.
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Rollout API")
               .WithTheme(ScalarTheme.DeepSpace)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.MapControllers();

// Ensure the database is up-to-date on startup. 
// Note: In large production environments, migrations are usually run as part of a separate CD step.
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<RolloutDbContext>();
dbContext.Database.Migrate();

app.Run();


public partial class Program { }