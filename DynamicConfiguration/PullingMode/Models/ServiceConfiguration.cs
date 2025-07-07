namespace DynamicConfigLab.DynamicConfiguration.PullingMode.Models;

public class ServiceConfiguration
{
    /// <summary>
    /// Partition Key
    /// e.g., "service#auth-api"
    /// </summary>
    public string pk { get; set; } = string.Empty;

    /// <summary>
    /// Sort Key 
    /// e.g., "config#global#Serilog:MinimumLevel:Default"
    /// </summary>
    public string sk { get; set; } = string.Empty;
    
    public string configValue { get; set; } = string.Empty;

    /// <summary>
    /// update timestamp (Unix epoch time)
    /// </summary>
    public long updatedAt { get; set; }

    /// <summary>
    /// deletion timestamp (Unix epoch time)。
    /// </summary>
    public long? deletedAt { get; set; }
}