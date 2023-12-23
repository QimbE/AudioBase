using Application;
using Carter;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Presentation;
using Serilog;

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
        
        builder.Services.AddSwaggerGen();

        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));
        
        builder.Services.AddCors();

        
        
        var app = builder.Build();

        //TODO: Add Global exception handling
        
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
        
        //TODO: app.UseAuthentication(); app.UseAuthorization();

        app.UseSerilogRequestLogging();

        //TODO: app.MapCarter(); app.MapGraphQL();
        
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