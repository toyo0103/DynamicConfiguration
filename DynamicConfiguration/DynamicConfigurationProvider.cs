using System.Collections.ObjectModel;
using DynamicConfigLab.DynamicConfiguration.Interfaces;
using Microsoft.Extensions.Primitives;

namespace DynamicConfigLab.DynamicConfiguration;

public class DynamicConfigurationProvider:ConfigurationProvider
{
    private readonly IDynamicConfigurationStore _store;
    public DynamicConfigurationProvider(IDynamicConfigurationStore store)
    {
        _store = store;
        Load();
        ChangeToken.OnChange(
            () => _store.GetReloadToken(),
            () => Load()
        );
    }
    public override void Load()
    {
        var storeData = _store.LoadAsync(CancellationToken.None).GetAwaiter().GetResult();
        // Synchronously retrieve dynamic values
        Data = new ReadOnlyDictionary<string, string>(
            storeData as IDictionary<string, string> ?? new Dictionary<string, string>(storeData)
        )!;
        OnReload();
    }
}