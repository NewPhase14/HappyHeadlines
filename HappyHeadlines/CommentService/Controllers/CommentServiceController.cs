using CommentDatabase;
using CoreDomain;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentServiceController : ControllerBase
{
    private Database database = Database.GetInstance();
    
    [HttpPost(Name = "CreateArticle")]
    public IActionResult CreateComment([FromBody] Comment comment)
    {
        database.InsertComment(comment);
        return Ok();
    }

    [HttpGet(Name = "GetComments")]
    public ActionResult<List<Comment>>GetComments()
    {
        return Ok(database.GetAllComments());
    }

    
}