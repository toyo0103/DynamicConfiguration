using DynamicConfigLab.DynamicConfiguration.PullingMode.Models;

namespace DynamicConfigLab.DynamicConfiguration.PullingMode.Interfaces;

public interface IConfigurationRepository
{
    Task<Dictionary<string, EffectiveConfigValue>> GetEffectiveConfigurationAsync(string serviceName, string scope);

    Task CreateOrUpdateConfigAsync(CreateOrUpdateConfigRequest request);

    Task<bool> DeleteConfigAsync(string serviceName, string scope, string configName);
}