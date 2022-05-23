using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using Tilde.MT.FileTranslationService.Interfaces.Facades;
using Tilde.MT.FileTranslationService.Models.Configuration;

namespace Tilde.MT.FileTranslationService.Services
{
    /// <summary>
    /// Removes old File metadata and linked files from storage after configured ttl
    /// </summary>
    public class TranslationCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly ILogger<TranslationCleanupService> _logger;
        private readonly ConfigurationSettings _configurationSettings;

        private readonly TimeSpan workInterval = TimeSpan.FromMinutes(1);

        private const string ReservedMetadataExpirationGroup = "__default__";

        public TranslationCleanupService(
            IServiceScopeFactory scopeFactory,
            IOptions<ConfigurationSettings> configurationSettings,
            ILogger<TranslationCleanupService> logger
        )
        {
            _logger = logger;
            _configurationSettings = configurationSettings.Value;
            _scopeFactory = scopeFactory;
        }

        protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cleaner start");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var translationFacade = scope.ServiceProvider.GetRequiredService<IFileTranslationFacade>();

                    _logger.LogInformation("Remove expired metadata");

                    var ttl = _configurationSettings.UserGroupMetadataExpiration[ReservedMetadataExpirationGroup];

                    await translationFacade.RemoveExpiredTasks(ttl);

                    _logger.LogInformation("Expired metadata removal completed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to remove expired metadata");
                }

                try
                {
                    await System.Threading.Tasks.Task.Delay(workInterval, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Waiting interruped");
                }
            }
            _logger.LogInformation("Cleaner stop");
        }
    }
}
