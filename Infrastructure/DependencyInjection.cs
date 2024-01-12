using System.Text;
using Application.Authentication;
using Infrastructure.Authentication;
using Application.DataAccess;
using Infrastructure.Authentication.Extensions;
using Infrastructure.BackgroundJobs;
using Infrastructure.Cache;
using Infrastructure.Data;
using Infrastructure.Email;
using Infrastructure.Idempotence;
using Infrastructure.Outbox;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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

            // Quartz devs misdesigned or miscoded the scheduler lifetime so we need different schedulers for multiple app instances
            configure.SchedulerName = Guid.NewGuid().ToString();
            
            configure
                .AddJob<ProcessOutboxMessagesJob>(jobKey)
                .AddTrigger(
                    trigger => trigger
                        .ForJob(jobKey)
                        .WithSimpleSchedule(
                            schedule => schedule
                                .WithIntervalInSeconds(30)
                                .RepeatForever()
                            )
                    );
        });

        services.AddQuartzHostedService();
        
        services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));

        services.AddScoped<IEmailSender, EmailSender>();
        
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