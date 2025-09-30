using Microsoft.AspNetCore.Mvc;

namespace MachineManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<object> Get()
    {
        _logger.LogInformation("Test endpoint called");
        return Ok(new { message = "API is working!", timestamp = DateTime.Now });
    }

    [HttpGet("version")]
    public ActionResult<object> GetVersion()
    {
        return Ok(new { version = "1.0.0", environment = "Development" });
    }
}