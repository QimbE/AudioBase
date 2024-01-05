using Application.DataAccess;
using Infrastructure.BackgroundJobs;
using Infrastructure.Cache;
using Infrastructure.Data;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using StackExchange.Redis;
using Throw;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>((provider, options) =>
            options
                .AddInterceptors(provider.GetRequiredService<InsertOutboxMessageInterceptor>())
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection")
                .ThrowIfNull()
                .Throw().IfNullOrWhiteSpace(x => x)
                )
            );

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        services.AddSingleton<InsertOutboxMessageInterceptor>();
        
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
        
        // Quartz
        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

            configure
                .AddJob<ProcessOutboxMessagesJob>(jobKey)
                .AddTrigger(
                    trigger => trigger
                        .ForJob(jobKey)
                        .WithSimpleSchedule(
                            schedule => schedule
                                .WithIntervalInSeconds(10)
                                .RepeatForever()
                            )
                    );
        });

        services.AddQuartzHostedService();
        
        return services;
    }
}