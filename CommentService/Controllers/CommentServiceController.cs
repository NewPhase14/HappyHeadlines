using CommentService.Service;
using CoreDomain.Dtos.CommentDtos;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentServiceController(ICommentService commentService) : ControllerBase
{
    [HttpPost(Name = "CreateComment")]
    public async Task<ActionResult<CommentResponseDto>> CreateComment([FromBody] CreateCommentDto createCommentDto)
    {
        var comment = await commentService.CreateComment(createCommentDto);
        return Ok(comment);
    }

    [HttpGet(Name = "GetComments")]
    public async Task<ActionResult<List<CommentResponseDto>>> GetComments()
    {
        return Ok(await commentService.GetAllComments());
    }

    [HttpGet("getCommentCache/{articleId}", Name = "GetCommentCache")]
    public async Task<ActionResult<List<CommentResponseDto>>> GetCommentCache(string articleId)
    {
        return Ok(await commentService.GetCommentCacheAsync(articleId));
    }

    [HttpGet("getCommentsByArticleId/{articleId}", Name = "GetCommentsByArticleId")]
    public async Task<ActionResult<List<CommentResponseDto>>> GetCommentsByArticleId(string articleId)
    {
        return Ok(await commentService.GetCommentsByArticleId(articleId));
    }
}