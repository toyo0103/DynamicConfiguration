using DynamicConfigLab.DynamicConfiguration.PollingMode.Interfaces;
using DynamicConfigLab.DynamicConfiguration.PollingMode.Models;
using Microsoft.AspNetCore.Mvc;

namespace DynamicConfigLab.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigurationController(IConfigurationRepository repository, ILogger<ConfigurationController> logger)
    : ControllerBase
{
    
    [HttpGet("{serviceName}")]
    public async Task<IActionResult> GetConfiguration(string serviceName, [FromQuery] string scope = "global")
    {
        try
        {
            var config = await repository.GetEffectiveConfigurationAsync(serviceName, scope);
            if (config.Count == 0)
            {
                return NotFound("No configurations found for the specified service and scope.");
            }
            return Ok(config);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error when getting configuration for service: {serviceName}", serviceName);
            return StatusCode(500, "An error occurred while retrieving the configuration.");
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> CreateOrUpdateConfig([FromBody] CreateOrUpdateConfigRequest request)
    {
        try
        {
            await repository.CreateOrUpdateConfigAsync(request);
            logger.LogInformation("Create or Update Successfully: {ServiceName}, {Scope}, {ConfigName}", request.ServiceName, request.Scope, request.ConfigName);
            return Ok(new { message = "Configuration saved successfully." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error when writing item into DynamoDB.");
            return StatusCode(500, "An error occurred while saving the configuration.");
        }
    }
    
    [HttpDelete("{serviceName}/{scope}/{configName}")]
    public async Task<IActionResult> DeleteConfig(string serviceName, string scope, string configName)
    {
        try
        {
            var success = await repository.DeleteConfigAsync(serviceName, scope, configName);
            if (!success)
            {
                return NotFound("The specified configuration to delete was not found.");
            }
            logger.LogInformation("delete successfully: {ServiceName}, {Scope}, {ConfigName}", serviceName, scope, configName);
            return NoContent(); 
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "error");
            return StatusCode(500, "An error occurred while deleting the configuration.");
        }
    }
}