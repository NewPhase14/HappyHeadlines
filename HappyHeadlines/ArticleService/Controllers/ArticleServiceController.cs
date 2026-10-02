using System.Diagnostics;
using ArticleService.Service;
using CoreDomain.Dtos.ArticleDtos;
using Microsoft.AspNetCore.Mvc;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace ArticleService.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticleServiceController(
    IArticleService articleService) : ControllerBase
{
    [HttpPost(Name = "CreateArticle")]
    public async Task<ActionResult<ArticleResponseDto>> CreateArticle(
        [FromBody] CreateArticleDto createArticleDto)
    {
        MonitoringService.ActivitySource.StartActivity();
        
        var article = await articleService.CreateArticle(createArticleDto);

        return Ok(article);
    }

    [HttpGet(Name = "GetAllArticles")]
    public async Task<ActionResult<List<ArticleResponseDto>>> GetAllArticles()
    {
        var articles = await articleService.GetAllArticles();

        return Ok(articles);
    }

    [HttpGet("getLatestArticles", Name = "GetLatestArticles")]
    public async Task<ActionResult<List<ArticleResponseDto>>> GetLatestArticles()
    {
        var articles = await articleService.GetLatestArticles();

        return Ok(articles);
    }

    [HttpGet("getRandomArticle", Name = "GetRandomArticle")]
    public async Task<ActionResult<ArticleResponseDto>> GetRandomArticle()
    {
        var article = await articleService.GetRandomArticle();

        return Ok(article);
    }

    [HttpGet("{articleId}", Name = "GetArticleById")]
    public async Task<ActionResult<ArticleResponseDto>> GetArticleById(string articleId)
    {
        var article = await articleService.GetArticleById(articleId);

        return Ok(article);
    }

    [HttpPut(Name = "UpdateArticle")]
    public async Task<ActionResult<ArticleResponseDto>> UpdateArticle([FromBody] UpdateArticleDto updateArticleDto)
    {
        var updatedArticle = await articleService.UpdateArticle(updateArticleDto);

        return Ok(updatedArticle);
    }

    [HttpDelete("{id}", Name = "DeleteArticle")]
    public async Task<IActionResult> DeleteArticle(string id)
    {
        await articleService.DeleteArticle(id);

        return Ok("Article deleted successfully");
    }
}