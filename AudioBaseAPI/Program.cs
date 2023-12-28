using Application;
using Carter;
using Infrastructure;
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
            .AddApplication(builder.Configuration)
            .AddInfrastructure(builder.Configuration)
            .AddPresentation();
        
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new()
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
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
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        //Cors policies
        if (app.Environment.IsDevelopment())
        {
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
        }

        app.UseHttpsRedirection();
        
        app.UseAuthentication();
        
        app.UseAuthorization();

        app.UseSerilogRequestLogging();

        //TODO: app.MapCarter();
        app.MapGraphQL();
        
        //Auto Db migration applier
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<ApplicationDbContext>();
            if (context.Database.GetMigrations().Any() && context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
        
        app.Run();
    }
}