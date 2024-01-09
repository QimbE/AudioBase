using System.Text;
using Application.Authentication.Extensions;
using Application.Behaviors;
using Application.ExceptionHandlers;
using Application.GraphQL;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddPipelineBehaviors();
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        // GraphQL configuration
        services.AddGraphQLServer()
            .ConfigureHotChocolateTypes()
            .UseDefaultPipeline();
        
        // Authentication
        var secretKey = Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!);
        
        var tokenValidationParameter = new TokenValidationParameters
        {
            ValidIssuer = configuration["JwtSettings:Issuer"],
            ValidAudience = configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = tokenValidationParameter;
        });
        
        services.AddAuthorization();

        services.AddRoleAuthorizationPolicies();
        
        return services;
    }
}