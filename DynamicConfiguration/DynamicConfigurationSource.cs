using DynamicConfigLab.DynamicConfiguration.Interfaces;

namespace DynamicConfigLab.DynamicConfiguration;

public class DynamicConfigurationSource(IDynamicConfigurationStore store) : IConfigurationSource
{
    public IDynamicConfigurationStore Store { get; } = store;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new DynamicConfigurationProvider(Store);
}