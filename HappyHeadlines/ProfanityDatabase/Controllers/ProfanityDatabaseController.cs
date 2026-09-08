using CoreDomain;
using Microsoft.AspNetCore.Mvc;

namespace ProfanityDatabase.Controllers;

[ApiController]
[Route("[controller]")]
public class ProfanityDatabaseController : ControllerBase
{
    private readonly Database database = Database.GetInstance();

    [HttpPost("recreate")]
    public IActionResult Recreate()
    {
        database.RecreateDatabase();

        return Ok();
    }

    [HttpDelete]
    public IActionResult Delete()
    {
        database.DeleteDatabase();

        return Ok();
    }

    [HttpPost("word")]
    public IActionResult AddWord([FromBody] Profanity profanity)
    {
        database.InsertWord(profanity);

        return Ok();
    }

    [HttpGet("words")]
    public ActionResult<List<string>> GetWords()
    {
        return Ok(database.GetWords());
    }
}