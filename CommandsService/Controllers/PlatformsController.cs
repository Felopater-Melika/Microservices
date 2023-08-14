using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    public PlatformsController() { }

    [HttpPost]
    public ActionResult<IEnumerable<string>> TestInBoundConnection()
    {
        Console.WriteLine("--> Inbound test connection...");
        return Ok("Connected");
    }
}
