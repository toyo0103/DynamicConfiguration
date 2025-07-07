using DynamicConfigLab.DynamicConfiguration.Interfaces;

namespace DynamicConfigLab.DynamicConfiguration;

public class DynamicConfigurationSource(IDynamicConfigurationStore store) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DynamicConfigurationProvider(store);
    }
}