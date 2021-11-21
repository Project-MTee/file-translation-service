using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Tilde.MT.FileTranslationService.Models.Configuration;

namespace Tilde.MT.FileTranslationService.Extensions
{
    public static class MessagingExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceConfiguration = configuration.GetSection("Services").Get<ConfigurationServices>();

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

            return services;
        }
    }
}
