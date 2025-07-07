using System.Collections.ObjectModel;
using DynamicConfigLab.DynamicConfiguration.Interfaces;
using DynamicConfigLab.DynamicConfiguration.PullingMode.Interfaces;
using DynamicConfigLab.Models;
using Microsoft.Extensions.Options;

namespace DynamicConfigLab.DynamicConfiguration.PullingMode;

public class ConfigPollingService(
    ILogger<ConfigPollingService> logger,
    IConfigurationRepository configurationRepository,
    IDynamicConfigurationStore configurationStore,
    IOptions<AppSettings> appSettingsOption)
    : IHostedService, IDisposable
{
    private Timer? _timer;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Config Polling Service is starting.");
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var serviceName = appSettingsOption.Value.Service;
        var serviceScope = appSettingsOption.Value.Cluster;

        try
        {
            var effectiveConfigs = configurationRepository.GetEffectiveConfigurationAsync(serviceName, serviceScope).GetAwaiter().GetResult();
            logger.LogInformation("Successfully fetched {count} effective configurations for service '{serviceName}'.", effectiveConfigs.Count, serviceName);

            var data =new ReadOnlyDictionary<string, string>( 
                effectiveConfigs.ToDictionary(s => s.Key, s => s.Value.Value));
            configurationStore.Reload(data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to poll and update configuration for service '{serviceName}'.", serviceName);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Config Polling Service is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
