using Microsoft.AspNetCore.Mvc;

namespace ProfanityService.Controllers;

[ApiController]
[Route("[controller]")]
public class ProfanityServiceController : ControllerBase
{
    private readonly ProfanityService profanityService = new ProfanityService();

    [HttpPost("checkProfanity")]
    public ActionResult<bool> CheckProfanity([FromBody] string message)
    {
        var containsProfanity = profanityService.ContainsProfanity(message);

        return Ok(containsProfanity);
    }
}