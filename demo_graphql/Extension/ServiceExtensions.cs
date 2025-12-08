using demo_graphql.BAL.IServices;
using demo_graphql.BAL.Services;
using demo_graphql.Controllers;
using demo_graphql.Models;
using FMS.Core.Models;
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
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(ConfigureSwaggerGen);
    }
    private static void ConfigureSwaggerGen(SwaggerGenOptions options)
    {
        // Add JWT Bearer security definition
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter your token"
        });

        // Apply security to all operations
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
            });
    }
}