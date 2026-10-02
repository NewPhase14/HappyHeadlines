using CommentCache.Service;
using CoreDomain;
using Microsoft.AspNetCore.Mvc;

namespace CommentCache.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentCacheController(ICommentCacheService commentCacheService) : ControllerBase
{
    [HttpGet("getCommentsByArticleId/{articleId}", Name = "GetCommentsByArticleId")]
    public async Task<List<Comment>> GetCommentsByArticleId(string articleId)
    {
        return await commentCacheService.GetCommentsByArticleId(articleId);
    }
}