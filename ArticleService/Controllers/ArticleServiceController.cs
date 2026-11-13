using ArticleService.Service;
using CoreDomain.Dtos.ArticleDtos;
using Microsoft.AspNetCore.Mvc;
using Monitoring;


namespace ArticleService.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticleServiceController(
    IArticleService articleService) : ControllerBase
{
    [HttpPost("createArticle", Name = "CreateArticle")]
    public async Task<ActionResult<ArticleResponseDto>> CreateArticle(
        [FromBody] CreateArticleDto createArticleDto)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();

        var article = await articleService.CreateArticle(createArticleDto);

        return Ok(article);
    }

    [HttpGet("getAllArticles", Name = "GetAllArticles")]
    public async Task<ActionResult<List<ArticleResponseDto>>> GetAllArticles()
    {
        var articles = await articleService.GetAllArticles();

        return Ok(articles);
    }

    [HttpGet("getArticlesFromLast14Days", Name = "GetArticlesFromLast14Days")]
    public async Task<ActionResult<List<ArticleResponseDto>>> GetArticlesFromLast14Days()
    {
        var articles = await articleService.GetArticlesFromLast14Days();

        return Ok(articles);
    }
    
    [HttpGet("getArticleById/{articleId}", Name = "GetArticleById")]
    public async Task<ActionResult<ArticleResponseDto>> GetArticleById(string articleId)
    {
        var article = await articleService.GetArticleById(articleId);

        return Ok(article);
    }

    [HttpPut("updateArticle", Name = "UpdateArticle")]
    public async Task<ActionResult<ArticleResponseDto>> UpdateArticle([FromBody] UpdateArticleDto updateArticleDto)
    {
        var updatedArticle = await articleService.UpdateArticle(updateArticleDto);

        return Ok(updatedArticle);
    }

    [HttpDelete("deleteArticle/{id}", Name = "DeleteArticle")]
    public async Task<IActionResult> DeleteArticle(string id)
    {
        await articleService.DeleteArticle(id);

        return Ok("Article deleted successfully");
    }

    [HttpGet("getLastestArticle", Name = "GetLatestArticle")]
    public async Task<ActionResult<ArticleResponseDto>> GetLatestArticle()
    {
        var article = await articleService.GetLatestArticle();

        return Ok(article);
    }
}