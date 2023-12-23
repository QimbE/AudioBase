using Application;
using Carter;
using Infrastructure;
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
            .AddInfrastructure()
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
        
        app.Run();
    }
}