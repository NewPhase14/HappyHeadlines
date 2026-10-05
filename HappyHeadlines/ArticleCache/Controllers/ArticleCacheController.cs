using ArticleCache.Service;
using Microsoft.AspNetCore.Mvc;

namespace ArticleCache.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticleCacheController(IArticleCacheService articleCacheService) : ControllerBase
{
    [HttpGet("getArticleById/{articleId}", Name = "GetArticleById")]
    public async Task<IActionResult> GetArticleById(string articleId)
    {
        var article = await articleCacheService.GetArticleById(articleId);
        if (article == null)
        {
            return NotFound();
        }
        return Ok(article);
    }
}