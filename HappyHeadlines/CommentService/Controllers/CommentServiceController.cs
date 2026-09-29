using CommentDatabase;
using CommentService.Service;
using CoreDomain;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentServiceController(ICommentService commentService) : ControllerBase
{
    [HttpPost(Name = "CreateComment")]
    public async Task<IActionResult> CreateComment([FromBody] Comment comment)
    {
        await commentService.CreateComment(comment);
        return Ok();
    }

    [HttpGet(Name = "GetComments")]
    public ActionResult<List<Comment>> GetComments()
    {
        return Ok(commentService.GetComments().Result);
    }
    
    [HttpGet("getCommentCache/{articleId}", Name = "GetCommentCache")]
    public async Task<ActionResult<List<Comment>>> GetCommentCache(string articleId)
    {
        return Ok(await commentService.GetCommentCacheAsync(articleId));
    }
    
    [HttpGet("getCommentsByArticleId/{articleId}", Name = "GetCommentsByArticleId")]
    public ActionResult<List<Comment>> GetCommentsByArticleId(string articleId)
    {
        return Ok(commentService.GetCommentsByArticleId(articleId).Result);
    }
}