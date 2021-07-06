using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;

namespace Tilde.MT.FileTranslationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureSerilog();

            var host = CreateHostBuilder(args).Build();

            MigrateDatabase(host);

            host.Run();
        }

        private static void MigrateDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                Log.Information("Migrate database");

                var context = services.GetRequiredService<Models.Database.TaskDbContext>();
                context.Database.Migrate();

                Log.Information("Migration completed");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred creating the DB.");
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = 20480; // 20KB
                    });
                });
        }

        private static void ConfigureSerilog()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
#if DEBUG
                .AddJsonFile($"appsettings.Development.json", optional: false)
#endif
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
