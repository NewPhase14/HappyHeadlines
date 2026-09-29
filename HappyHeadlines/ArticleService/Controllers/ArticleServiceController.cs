using System.Diagnostics;
using ArticleDatabase;
using ArticleService.Interface;
using CoreDomain;
using Microsoft.AspNetCore.Mvc;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Serilog;

namespace ArticleService.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticleServiceController(IArticleService articleService, TraceContextPropagator traceContextPropagator) : ControllerBase
{
    [HttpPost(Name = "CreateArticle")]
    public async Task<IActionResult> CreateArticle([FromBody] Article article)
    {
        // Extract context sent from Publisher
        var parentContext = traceContextPropagator.Extract(
            default,
            Request,
            (request, key) => request.Headers[key].ToArray());

        Baggage.Current = parentContext.Baggage;
        
        using var activity = MonitoringService.ActivitySource.StartActivity(
            "ArticleService.CreateArticle", 
            ActivityKind.Consumer, 
            parentContext.ActivityContext);
        
        await articleService.CreateArticle(article);
        return Ok();
    }
    
    [HttpGet("getLatestArticles", Name = "GetLatestArticles")]
    public async Task<ActionResult<List<Article>>> GetLatestArticles()
    {
        var articles = await articleService.GetLatestArticles();
        return Ok(articles);
    }

    [HttpGet("getRandomArticle", Name = "GetRandomArticle")]
    public ActionResult<Article> GetRandomArticle()
    {
        var articles = articleService.GetArticles();
        return Ok(articles);
    }

    [HttpGet(Name = "GetArticles")]
    public ActionResult<List<Article>> GetArticles()
    {
        return Ok(articleService.GetArticles());
    }

    [HttpDelete("{id}", Name = "DeleteArticle")]
    public IActionResult DeleteArticle(string id)
    {
        articleService.DeleteArticle(id);
        return Ok("Article deleted successfully");
    }

    [HttpPut("{id}", Name = "UpdateArticle")]
    public IActionResult UpdateArticle(string id, [FromBody] Article article)
    {
        articleService.UpdateArticle(id, article);
        return Ok("Article updated successfully");
    }
    
    [HttpGet("getArticleById/{articleId}", Name = "GetArticleById")]
    public async Task<IActionResult> GetArticleById(string articleId)
    {
        var article = await articleService.GetArticleById(articleId);
        return Ok(article);
    }
}