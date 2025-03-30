using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Ansible.Host;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Starting Ansible API");
            var builder = WebApplication.CreateBuilder(args);
            
            // Configure with environment variables and JSON
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables("ANSIBLE_")
                .AddCommandLine(args);

            // Add services from API project
            var app = API.Startup.ConfigureApp(args);
            
            app.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application terminated unexpectedly: {ex.Message}");
        }
    }
}