using CoreDomain;
using DraftDatabase;
using Microsoft.AspNetCore.Mvc;

namespace DraftService.Controllers;

[ApiController]
[Route("[controller]")]
public class DraftServiceController : ControllerBase
{
    private Database database = Database.GetInstance();
    
    [HttpPost("createDraft", Name = "CreateDraft")]
    public IActionResult CreateDraft([FromBody] Article article)
    {
        database.InsertDraft(article);
        return Ok();
    }

    [HttpGet(Name = "GetDrafts")]
    public ActionResult<List<Article>>GetDrafts()
    {
        return Ok(database.GetAllDrafts());
    }

    [HttpDelete("{id}", Name = "DeleteDraft")]
    public IActionResult DeleteDraft(string id)
    {
        database.DeleteDraft(id);
        return Ok("Draft deleted successfully");
    }

    [HttpPut("{id}", Name = "UpdateDraft")]
    public IActionResult UpdateDraft(string id, [FromBody] Article article)
    {
        //TODO: If draft is updates from draft to non-draft, it should be deleted from DraftDatabase and inserted into ArticleDatabase.
        
        database.UpdateDraft(id, article);
        return Ok("Draft updated successfully");
    }

}