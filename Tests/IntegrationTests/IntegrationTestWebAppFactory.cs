using System.Data.Common;
using AudioBaseAPI;
using Infrastructure.Data;
using Infrastructure.Outbox;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Respawn;
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
    
    private Respawner _respawner = default!;

    private DatabaseFacade _databaseFacade = default!;
    
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

            // Removing whole background task infrastructure
            var backgroundTaskDescriptor = services
                .SingleOrDefault(s => s.ServiceType == typeof(IHostedService) &&
                                      s.ImplementationType == typeof(QuartzHostedService)
                                      );

            if (backgroundTaskDescriptor is not null)
            {
                services.Remove(backgroundTaskDescriptor);
            }
            
            services.AddSingleton<IConnectionMultiplexer>(sp => 
                ConnectionMultiplexer.Connect(
                    new ConfigurationOptions
                    {
                        EndPoints = {_redisContainer.GetConnectionString()},
                        AbortOnConnectFail = false
                    }
                ));
            
            var provider = services.BuildServiceProvider();
            var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            _databaseFacade = context.Database;
            _databaseFacade.Migrate();

            InitRespawner().Wait();
        });
    }
    
    public async Task InitRespawner()
    {
        DbConnection conn = await GetOpenedDbConnectionAsync();
        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            SchemasToInclude = ["public"],
            TablesToIgnore = ["__EFMigrationsHistory", "Roles", "ReleaseTypes"],
            DbAdapter = DbAdapter.Postgres,
            WithReseed = true
        });
    }
    
    public async Task ResetDatabaseAsync()
    {
        DbConnection conn = await GetOpenedDbConnectionAsync();
        await _respawner.ResetAsync(conn);
    }

    private async Task<DbConnection> GetOpenedDbConnectionAsync()
    {
        var conn = _databaseFacade.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
            await conn.OpenAsync();
        return conn;
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await _redisContainer.StopAsync();
        await _redisContainer.DisposeAsync();
    }
}