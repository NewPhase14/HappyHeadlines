using ArticleCache.Service;
using Microsoft.AspNetCore.Mvc;

namespace ArticleCache.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticleCacheController(IArticleCacheService articleCacheService) : ControllerBase
{
    [HttpGet(Name = "GetLatestArticles")]
    public async Task<IActionResult> GetLatestArticles()
    {
        await articleCacheService.GetLatestArticlesAsync();
        return Ok();
    }
    
    [HttpGet("getArticleAsync/{articleId}", Name = "GetRandomArticle")]
    public async Task<IActionResult> GetArticleAsync(string articleId)
    {
        var article = await articleCacheService.GetArticleAsync(articleId);
        return Ok(article);
    }
}