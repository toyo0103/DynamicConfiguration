using DynamicConfigLab.DynamicConfiguration.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DynamicConfigLab.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController(IDynamicConfigurationStore store) : ControllerBase
{
    // POST api/config/{key}/{value}
    [HttpPost("{key}/{value}")]
    public IActionResult Set(string key, string value)
    {
        store.Set(key, value);
        store.SignalChange();
        return Ok(new { key, value });
    }

    // DELETE api/config/{key}
    [HttpDelete("{key}")]
    public IActionResult Remove(string key)
    {
        store.Remove(key);
        store.SignalChange();
        return Ok(key);
    }
}