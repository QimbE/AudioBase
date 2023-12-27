using Application.Behaviors;
using Application.ExceptionHandlers;
using Application.GraphQL;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
            configuration.AddPipelineBehaviors();
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        // GraphQL configuration
        services.AddGraphQLServer()
            .ConfigureHotChocolateTypes()
            .ConfigurePipeline();
        
        return services;
    }
}