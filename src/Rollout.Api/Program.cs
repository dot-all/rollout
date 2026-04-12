using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rollout.Application.Common.Interfaces;
using Rollout.Application.Common.Pipeline;
using Rollout.Application.Features.FeatureFlags.Create;
using Rollout.Infrastructure.Persistence;
using Rollout.Infrastructure.Persistence.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddMediatR(typeof(CreateFeatureFlagCommand).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateFeatureFlagCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped<IFeatureFlagRepository, FeatureFlagRepository>();

builder.Services.AddDbContext<RolloutDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); 
    
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Rollout API")
               .WithTheme(ScalarTheme.DeepSpace)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.MapControllers();

app.Run();
