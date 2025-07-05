using System.Collections.ObjectModel;
using DynamicConfigLab.DynamicConfiguration.Interfaces;
using Microsoft.Extensions.Primitives;

namespace DynamicConfigLab.DynamicConfiguration.ConfigurationStores;

internal class InMemoryDynamicStore : IDynamicConfigurationStore
{
    private readonly Dictionary<string, string> _data = new();
    private CancellationTokenSource _cts = new();

    public Task<IReadOnlyDictionary<string, string>> LoadAsync(CancellationToken cancellationToken = default)
    {
        // Return a copy to avoid external mutation
        var readOnly = new ReadOnlyDictionary<string, string>(_data);
        return Task.FromResult<IReadOnlyDictionary<string, string>>(readOnly);
    }

    public IChangeToken GetReloadToken()
    {
        // Wrap the CancellationTokenSource's token
        return new CancellationChangeToken(_cts.Token);
    }

    /// <summary>
    /// Adds or updates a key, then signals change
    /// </summary>
    public void Set(string key, string value)
    {
        _data[key] = value;
    }

    /// <summary>
    /// Removes a key, then signals change
    /// </summary>
    public void Remove(string key)
    {
        _data.Remove(key);
    }

    public void SignalChange()
    {
        var previous = _cts;
        _cts = new CancellationTokenSource();
        previous.Cancel();
    }
}