using Microsoft.AspNetCore.Mvc;

namespace DynamicConfigLab.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoggingController(ILogger<LoggingController> logger, IConfiguration config) : ControllerBase
{
    // GET api/logging/test
    [HttpGet("test")]
    public IActionResult Test()
    {
        // Log at all levels
        logger.LogTrace("[Trace] Current Serilog default level is {Level}", config["Serilog:MinimumLevel:Default"]);
        logger.LogDebug("[Debug] Testing debug log");
        logger.LogInformation("[Info] Testing information log");
        logger.LogWarning("[Warn] Testing warning log");
        logger.LogError("[Error] Testing error log");
        return Ok();
    }
}