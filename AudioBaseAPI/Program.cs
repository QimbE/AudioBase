using Application;
using Carter;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Presentation;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

namespace AudioBaseAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services
            .AddApplication()
            .AddInfrastructure(builder.Configuration)
            .AddPresentation();
        
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new()
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            
            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));
        
        builder.Services.AddCors();

        
        
        var app = builder.Build();

        app.UseExceptionHandler(_ => { });
        
        app.ApplyMigrations();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );
        }
        else
        {
            app.UseCors(x => x
                .WithOrigins(
                    app.Configuration.GetValue<string[]>("Cors:AllowedHosts")!
                    )
            );
            // TODO: Add request rate limiter
        }

        app.UseHttpsRedirection();
        
        app.UseSerilogRequestLogging();
        
        app.UseAuthentication();
        
        app.UseAuthorization();

        app.MapCarter();
        
        app.MapGraphQL();
        
        app.Run();
    }
}

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using ApplicationDbContext dbContext = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
        
        dbContext.Database.Migrate();
    }
}