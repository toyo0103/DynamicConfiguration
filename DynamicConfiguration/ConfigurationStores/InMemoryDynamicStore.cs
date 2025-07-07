using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using DynamicConfigLab.DynamicConfiguration.Interfaces;
using Microsoft.Extensions.Primitives;

namespace DynamicConfigLab.DynamicConfiguration.ConfigurationStores;

internal class InMemoryDynamicStore : IDynamicConfigurationStore
{
    private ConcurrentDictionary<string, string> _data = new();
    private ConfigurationReloadToken _reloadToken = new();

    public Task<IReadOnlyDictionary<string, string>> LoadAsync(CancellationToken cancellationToken = default)
    {
        // Return a copy to avoid external mutation
        var readOnly = new ReadOnlyDictionary<string, string>(_data);
        return Task.FromResult<IReadOnlyDictionary<string, string>>(readOnly);
    }

    public IChangeToken GetReloadToken()
    {
        return _reloadToken;
    }

    /// <summary>
    /// Adds or updates a key, then signals change
    /// </summary>
    public void Set(string key, string value)
    {
        _data[key] = value;
        SignalChange();
    }

    /// <summary>
    /// Removes a key, then signals change
    /// </summary>
    public void Remove(string key)
    {
        _data.Remove(key, out _);
        SignalChange();
    }

    public void Reload(ReadOnlyDictionary<string, string> data)
    {
        _data = new (data);
        SignalChange();
    }
    
    private void SignalChange()
    {
        var previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
        previousToken.OnReload();
    }
}