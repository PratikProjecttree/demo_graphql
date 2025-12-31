using demo_graphql.BAL.IServices;
using demo_graphql.BAL.Services;
using demo_graphql.Controllers;
using demo_graphql.Models;
using demo_graphql.Services;
using FMS.Core.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public static class ServiceExtensions
{
    public static void ConfigureAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GraphQLConfigurationModel>(configuration.GetSection("GraphQLConfiguration"));
        services.Configure<SsoApiModel>(configuration.GetSection("SsoConfiguration"));
    }
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDapperService, DapperService>();
        services.AddScoped<IGLService, GLService>();
        services.AddScoped<IHasuraService, HasuraService>();
        services.AddScoped<IWorkFlowService, WorkFlowService>();
        services.AddScoped<IValidationService, ValidationService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IASMService, ASMService>();
        services.AddScoped<IMISService, MISService>();
        services.AddScoped<IEmailQueueService, EmailQueueService>();
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(ConfigureSwaggerGen);
    }
    private static void ConfigureSwaggerGen(SwaggerGenOptions options)
    {
        // Add JWT Bearer security definition
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your token"
        });
        // 2) NEW: X-App-Position header
        options.AddSecurityDefinition("Position", new OpenApiSecurityScheme
        {
            Name = "X-App-Position",
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Description = "Position Id header"
        });

        // Apply security to all operations
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Position"
                }
            },
            Array.Empty<string>()
        }
            });
    }
}