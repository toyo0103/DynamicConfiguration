namespace DynamicConfigLab.DynamicConfiguration.PollingMode.Models;

public class CreateOrUpdateConfigRequest
{
    public string ServiceName { get; set; } = string.Empty;
    public string Scope { get; set; } = "global"; 
    public string ConfigName { get; set; } = string.Empty;
    public string ConfigValue { get; set; } = string.Empty;
}