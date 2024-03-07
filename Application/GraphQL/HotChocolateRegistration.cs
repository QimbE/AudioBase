using Application.GraphQL.TypeConfigurations;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.GraphQL;

public static class HotChocolateRegistration
{
    public static IRequestExecutorBuilder ConfigureHotChocolateTypes(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddQueryType<Endpoint>()
            .AddType<RoleType>()
            .AddType<UserType>()
            .AddType<ReleaseTypeType>()
            .AddType<GenreType>()
            .AddType<ArtistType>()
            .AddProjections()
            .AddSorting()
            .AddFiltering()
            .AddAuthorization();
    }

    /// <summary>
    /// Configures default pipeline with caching
    /// </summary>
    /// <param name="builder">pipeline builder</param>
    /// <returns></returns>
    [Obsolete("Obsolete due to unexpected caching behavior. There is a chance to fix it if we somehow manage to deal with hotchocolate pipeline", true)]
    public static IRequestExecutorBuilder ConfigurePipeline(this IRequestExecutorBuilder builder)
    {
        return builder
            .UseInstrumentation()
            .UseExceptions()
            .UseTimeout()
            .UseDocumentCache()
            .UseDocumentParser()
            .UseDocumentValidation()
            .UseRequest<CachingMiddleware>()
            .UseOperationCache()
            .UseOperationComplexityAnalyzer()
            .UseOperationResolver()
            .UseOperationVariableCoercion()
            .UseOperationExecution();
    }
}