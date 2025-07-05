using Microsoft.Extensions.Primitives;

namespace DynamicConfigLab.DynamicConfiguration.Interfaces;

public interface IDynamicConfigurationStore
{
    /// <summary>
    /// Load all dynamic key/value pairs
    /// </summary>
    Task<IReadOnlyDictionary<string, string>> LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Change token that fires when store data changes
    /// </summary>
    IChangeToken GetReloadToken();

    /// <summary>
    /// Adds or updates a key.
    /// </summary>
    void Set(string key, string value);

    /// <summary>
    /// Removes a key.
    /// </summary>
    void Remove(string key);

    /// <summary>
    /// Manually triggers change token.
    /// </summary>
    void SignalChange();
}