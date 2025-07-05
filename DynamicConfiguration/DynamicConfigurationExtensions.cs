using DynamicConfigLab.DynamicConfiguration.ConfigurationStores;
using DynamicConfigLab.DynamicConfiguration.Interfaces;

namespace DynamicConfigLab.DynamicConfiguration;

public static class DynamicConfigurationExtensions
{
    public static WebApplicationBuilder AddDynamicConfiguration(
        this WebApplicationBuilder builder)
    {
        var store = new InMemoryDynamicStore();
        builder.Services.AddSingleton<IDynamicConfigurationStore>(store);
        builder.Configuration.Sources.Add(new DynamicConfigurationSource(store));
        return builder;
    }
}
