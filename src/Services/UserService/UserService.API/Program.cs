using BuildingBlocks.Tenancy;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.API.Endpoints;
using UserService.Application.Common.Behaviors;
using UserService.Application.Common.Settings;
using UserService.Application.Users.Commands.RegisterUser;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Options
builder.Services
    .AddOptions<DatabaseSettings>()
    .BindConfiguration(DatabaseSettings.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Tenancy
builder.Services.AddTenancy();

// EF Core
var connectionString = builder.Configuration
    .GetSection(DatabaseSettings.SectionName)
    .GetValue<string>(nameof(DatabaseSettings.ConnectionString))!;

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserHandler).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserHandler).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseTenancy();

app.MapUserEndpoints();
app.MapGet("/", () => "UserService running");

app.Run();
