using Ansible.Domain.Interfaces;
using Ansible.Infrastructure.Data;
using Ansible.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System;
using System.Net.Http;

namespace Ansible.API;

public static class Startup
{
    public static WebApplication ConfigureApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        ConfigureServices(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline
        ConfigurePipeline(app);

        return app;
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        // Add controllers
        builder.Services.AddControllers();
        
        // Add OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        // Add Health Checks
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<AnsibleDbContext>();
        
        // Add Serilog
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console());
        
        // Add OpenTelemetry
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddSource("Ansible.API")
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Ansible.API"))
                    .AddAspNetCoreInstrumentation();
            });
        
        // Add Database
        builder.Services.AddDbContext<AnsibleDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("Ansible.Infrastructure")
            ));
        
        // Register domain services
        RegisterDomainServices(builder.Services);
        
        // Register infrastructure services
        RegisterInfrastructureServices(builder.Services);
        
        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.MapControllers();
        
        // Map health check endpoint
        app.MapHealthChecks("/health");
    }

    private static void RegisterDomainServices(IServiceCollection services)
    {
        // No domain services to register at this time
    }

    private static void RegisterInfrastructureServices(IServiceCollection services)
    {
        // Register services
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<ITagService, TagService>();
        
        // Register HttpClient
        services.AddHttpClient("SpaceportApi", client =>
        {
            client.BaseAddress = new Uri("https://your-spaceport-api.ondigitalocean.app/");
        }).AddPolicyHandler(GetRetryPolicy());
    }
    
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Polly.Extensions.Http.HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}