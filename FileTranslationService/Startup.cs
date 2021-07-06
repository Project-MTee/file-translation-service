using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using Tilde.MT.FileTranslationService.Models.Configuration;
using Tilde.MT.FileTranslationService.Models.Mappings;
using Tilde.MT.FileTranslationService.Services;
using Tilde.MT.FileTranslationService.Enums;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json.Serialization;
using Tilde.MT.FileTranslationService.Facades;
using MassTransit;
using RabbitMQ.Client;
using Tilde.MT.FileTranslationService.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tilde.MT.FileTranslationService.Models;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Tilde.MT.FileTranslationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string DevelopmentCorsPolicy = "development-policy";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceConfiguration = Configuration.GetSection("Services").Get<ConfigurationServices>();
            services.Configure<ConfigurationServices>(Configuration.GetSection("Services"));
            services.Configure<ConfigurationSettings>(Configuration.GetSection("Configuration"));

            services.AddHttpClient();
            services.AddMemoryCache();

            services.AddCors(options =>
            {
                options.AddPolicy(name: DevelopmentCorsPolicy,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:4200").AllowAnyHeader();
                                  });
            });

            services.AddMvc().AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FileTranslationService", Version = "v1" });

                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });

                c.EnableAnnotations();

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{nameof(Tilde)}.{nameof(Tilde.MT)}.{nameof(Tilde.MT.FileTranslationService)}.xml"));
            });

            services.ConfigureSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerAuthenticationFilter>();
            });

            services.AddAuthentication()
                .AddBasicAuthentication(AuthenticationSchemeType.FileTranslationWorkflow, options => { }, credentials =>
                {
                    return Task.FromResult(
                        credentials.username == serviceConfiguration.FileTranslation.UserName &&
                        credentials.password == serviceConfiguration.FileTranslation.Password
                    );
                });

            services.AddDbContextPool<Models.Database.TaskDbContext>(options => {
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

            services.AddScoped<FileStorageService>();
            services.AddScoped<TaskService>();

            services.AddScoped<FileTranslationFacade>();

            services.AddHostedService<TranslationCleanupService>();
            services.AddSingleton<LanguageDirectionService>();

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddRequestClient<Models.RabbitMQ.FileTranslationRequest>();

                x.UsingRabbitMq((context, config) =>
                {
                    config.Host(serviceConfiguration.RabbitMQ.Host, "/", host =>
                    {
                        host.Username(serviceConfiguration.RabbitMQ.UserName);
                        host.Password(serviceConfiguration.RabbitMQ.Password);
                    });

                    #region File translation publish configuration

                    // Specify queue
                    //EndpointConvention.Map<Models.RabbitMQ.FileTranslationRequest>(new Uri("queue:file-translation"));
                    
                    // Specify exchange 
                    config.Message<Models.RabbitMQ.FileTranslationRequest>(x =>
                    {
                        x.SetEntityName("file-translation");
                    });

                    // Set exchange options
                    config.Publish<Models.RabbitMQ.FileTranslationRequest>(x =>
                    {
                        x.ExchangeType = ExchangeType.Fanout;
                        x.Durable = true;
                    });

                    // Set message attributes
                    config.Send<Models.RabbitMQ.FileTranslationRequest>(x =>
                    {
                        x.UseRoutingKeyFormatter(context =>
                        {
                            return $"file-translation";
                        });
                    });

                    #endregion

                    config.ConfigureEndpoints(context);

                    config.UseRawJsonSerializer(
                        MassTransit.Serialization.RawJsonSerializerOptions.AddTransportHeaders
                    );
                });
            });

            services.AddMassTransitHostedService(false);

            // Catch client errors
            services.Configure<ApiBehaviorOptions>(options => {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var modelStateEntries = actionContext.ModelState.Where(e => e.Value.Errors.Count > 0).ToArray();
                    var requestTooLarge = modelStateEntries.Where(item =>
                    {
                        return item.Value.Errors.Where(err => err.ErrorMessage.Contains("Request body too large")).Any();
                    }).Any();

                    if (requestTooLarge)
                    {
                        actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.RequestEntityTooLarge;

                        return new JsonResult(
                            new APIError()
                            {
                                Error = new Error()
                                {
                                    Code = ((int)HttpStatusCode.RequestEntityTooLarge) * 1000 + (int)Enums.ErrorSubCode.GatewayRequestTooLarge,
                                    Message = Enums.ErrorSubCode.GatewayRequestTooLarge.Description()
                                }
                            }
                        ); 
                    }
                    else
                    {
                        return new BadRequestObjectResult(
                            new APIError()
                            {
                                Error = new Error()
                                {
                                    Code = ((int)HttpStatusCode.BadRequest) * 1000 + (int)Enums.ErrorSubCode.GatewayRequestValidation,
                                    Message = Enums.ErrorSubCode.GatewayRequestValidation.Description()
                                }
                            }
                        );
                    }
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

#if DEBUG
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileTranslationService v1"));
#endif

            // Catch all unexpected errors
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        string response;
                        if (contextFeature.Error.Message.Contains("Request body too large"))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.RequestEntityTooLarge;
                            Log.Error($"Request too large: {contextFeature.Error}");

                            response = JsonSerializer.Serialize(new APIError()
                            {
                                Error = new Error()
                                {
                                    Code = ((int)HttpStatusCode.RequestEntityTooLarge) * 1000 + (int)Enums.ErrorSubCode.GatewayRequestTooLarge,
                                    Message = Enums.ErrorSubCode.GatewayRequestTooLarge.Description()
                                }
                            });
                        }
                        else
                        {
                            Log.Error($"Unexpected error: {contextFeature.Error}");
                            response = JsonSerializer.Serialize(new APIError()
                            {
                                Error = new Error()
                                {
                                    Code = ((int)HttpStatusCode.InternalServerError) * 1000 + (int)Enums.ErrorSubCode.GatewayGeneric,
                                    Message = Enums.ErrorSubCode.GatewayGeneric.Description() 
                                }
                            });
                        }

                        await context.Response.WriteAsync(response);
                    }
                });
            });

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
#if DEBUG
            app.UseCors(DevelopmentCorsPolicy);
#endif
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // Startup probe / Readyness probe
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                {
                    // check if MassTransit can connect 
                    Predicate = (check) => {
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
