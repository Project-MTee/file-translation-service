using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Models.Configuration;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;

namespace Tilde.MT.FileTranslationService.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddServiceAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceConfiguration = configuration.GetSection("Services").Get<ConfigurationServices>();

            services
                .AddAuthentication()
                .AddBasicAuthentication(AuthenticationScheme.FileTranslationWorkflow, 
                    options =>
                    {
                        options.Events = new BasicAuthenticationEvents
                        {
                            OnValidatePrincipal = context =>
                            {
                                var userNameMatches = context.UserName == serviceConfiguration.FileTranslation.UserName;
                                var passwordMatches = context.Password == serviceConfiguration.FileTranslation.Password;
                                if (userNameMatches && passwordMatches)
                                {
                                    var claims = new List<Claim>
                                    {
                                        new Claim(ClaimTypes.Name, context.UserName, context.Options.ClaimsIssuer)
                                    };

                                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                                    context.Principal = principal;
                                }
                                else
                                {
                                    // optional with following default.
                                    // context.AuthenticationFailMessage = "Authentication failed."; 
                                }

                                return Task.CompletedTask;
                            }
                        };
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
