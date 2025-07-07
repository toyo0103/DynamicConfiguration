using DynamicConfigLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DynamicConfigLab.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeatureController(IOptionsMonitor<FeatureSettings> monitor) : ControllerBase
{
    // GET api/feature
    [HttpGet]
    public ActionResult<FeatureSettings> Get()
    {
        // Return current settings
        return Ok(monitor.CurrentValue);
    }
}