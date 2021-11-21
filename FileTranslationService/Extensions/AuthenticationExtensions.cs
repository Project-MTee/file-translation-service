using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Models.Configuration;

namespace Tilde.MT.FileTranslationService.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddServiceAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceConfiguration = configuration.GetSection("Services").Get<ConfigurationServices>();

            services.AddAuthentication()
                .AddBasicAuthentication(AuthenticationScheme.FileTranslationWorkflow, options => { }, credentials =>
                {
                    return Task.FromResult(
                        credentials.username == serviceConfiguration.FileTranslation.UserName &&
                        credentials.password == serviceConfiguration.FileTranslation.Password
                    );
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(
                        AuthenticationScheme.FileTranslationWorkflow
                    )
                    .Build();
            });

            return services;
        }
    }
}
