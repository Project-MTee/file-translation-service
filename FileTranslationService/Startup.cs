using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Tilde.MT.FileTranslationService.Extensions;
using Tilde.MT.FileTranslationService.Facades;
using Tilde.MT.FileTranslationService.Interfaces.Facades;
using Tilde.MT.FileTranslationService.Interfaces.Services;
using Tilde.MT.FileTranslationService.Models.Configuration;
using Tilde.MT.FileTranslationService.Models.Mappings;
using Tilde.MT.FileTranslationService.Services;

namespace Tilde.MT.FileTranslationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ConfigurationServices>(Configuration.GetSection("Services"));
            services.Configure<ConfigurationSettings>(Configuration.GetSection("Configuration"));

            services.AddHttpClient();
            services.AddMemoryCache();

            services.AddCorsPolicies();

            services.AddMvc().AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddControllers();

            services.AddDocumentation();

            services.AddServiceAuthentication(Configuration);

            services.AddDbContextPool<Models.Database.TaskDbContext>(options =>
            {
                var serviceConfiguration = Configuration.GetSection("Services").Get<ConfigurationServices>();
                options.UseMySql(
                    serviceConfiguration.Database.ConnectionString,
                    ServerVersion.AutoDetect(serviceConfiguration.Database.ConnectionString)
                );
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            var mappingConfig = new MapperConfiguration(config =>
            {
                config.AddProfile(new MappingProfile());
            });
            services.AddSingleton(mappingConfig.CreateMapper());

            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IFileTranslationFacade, FileTranslationFacade>();
            services.AddScoped<ITaskTranslationService, TaskTranslationService>();

            services.AddHostedService<TranslationCleanupService>();
            services.AddSingleton<ILanguageDirectionService, LanguageDirectionService>();
            services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

            services.AddMessaging(Configuration);

            services.AddClientErrorProcessing();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

#if DEBUG
            app.UseDeveloperExceptionPage();

            app.UseDocumentation();
#endif

            app.UseUnhandledExceptionProcessing();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
#if DEBUG
            app.UseCorsPolicies();
#endif

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Disposition", "attachment; filename=\"api.json\"");
                await next.Invoke();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // Startup probe / Readyness probe
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                {
                    // check if MassTransit can connect 
                    Predicate = (check) =>
                    {
                        return check.Tags.Contains("ready");
                    }
                });

                // Liveness 
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions()
                {

                });
            });
        }
    }
}
