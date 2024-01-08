using AudioBaseAPI;
using Infrastructure.Data;
using Infrastructure.Outbox;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace IntegrationTests;

/// <summary>
/// AudioBaseAPI instances provider. Thread and data access safe (kinda).
/// </summary>
public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("mydatabase")
        .WithUsername("postgres")
        .WithPassword("mysecretpassword")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var dbContextOptionsDescriptor =
                services.SingleOrDefault(
                    s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
                    );
            
            if (dbContextOptionsDescriptor is not null)
            {
                // Removing actual dbcontext to replace it with one connected to test db container
                services.Remove(dbContextOptionsDescriptor);
            }

            services.AddDbContext<ApplicationDbContext>((provider, options) =>
                options
                    .AddInterceptors(provider.GetRequiredService<InsertOutboxMessageInterceptor>())
                    .UseNpgsql(_dbContainer.GetConnectionString())
                );

            var connectionMultiplexerDescriptor =
                services.SingleOrDefault(
                    s => s.ServiceType == typeof(IConnectionMultiplexer)
                    );

            if (connectionMultiplexerDescriptor is not null)
            {
                // Removing actual connectionmultiplexer to replace it with one connected to test redis container
                services.Remove(connectionMultiplexerDescriptor);
            }
            
            services.AddSingleton<IConnectionMultiplexer>(sp => 
                ConnectionMultiplexer.Connect(
                    new ConfigurationOptions
                    {
                        EndPoints = {_redisContainer.GetConnectionString()},
                        AbortOnConnectFail = false
                    }
                ));
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
    }
}