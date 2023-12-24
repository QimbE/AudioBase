using Application.DataAccess;
using Infrastructure.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Throw;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")
                .ThrowIfNull()
                .Throw().IfNullOrWhiteSpace(x => x)));

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        // Redis connection
        services.AddSingleton<IConnectionMultiplexer>(sp => 
            ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = {configuration.GetConnectionString("CacheConnection")
                        .ThrowIfNull()
                        .Throw().IfNullOrWhiteSpace(x => x)},
                    AbortOnConnectFail = false
                }
                ));

        // Cache service
        services.AddScoped<ICacheService, RedisCacheService>();
        
        return services;
    }
}