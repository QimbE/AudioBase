using Application.Authentication;
using Infrastructure.Authentication;
using Application.DataAccess;
using Infrastructure.BackgroundJobs;
using Infrastructure.Cache;
using Infrastructure.Data;
using Infrastructure.Idempotence;
using Infrastructure.Outbox;
using MediatR;
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
        // Db context configuration
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

        services.AddScoped<IHashProvider, HashProvider>();

        services.AddScoped<IJwtProvider, JwtProvider>();
        
        // Quartz background tasks
        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

            // Quartz devs missdesigned or misscoded the scheduler lifetime so we need different schedulers for multipule app instances
            configure.SchedulerName = Guid.NewGuid().ToString();
            
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

        // Decorating all notification handlers to be idempotent
        // TODO: This probably does not work, because i've highly likely missdesigned DomainEvent :D (See next lines to know how to fix)
        // Extract IDomainEvent interface implementing INotification, also do this with INotificationHandler
        // Replace all DomainEvent references in infrastructure layer with interface (don't forget actual handlers)
        // ...
        // Profit!
        services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));
        
        return services;
    }
}