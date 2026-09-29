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
    private Database database = Database.GetInstance();
    
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
        var articles = database.GetAllArticles();
        if (articles.Count == 0)
        {
            return NotFound("No articles found.");
        }

        var random = new Random();
        var newestArticle = articles[random.Next(articles.Count)];
        

        return Ok(newestArticle);
    }

    [HttpGet(Name = "GetArticles")]
    public ActionResult<List<Article>> GetArticles()
    {
        return Ok(database.GetAllArticles());
    }

    [HttpDelete("{id}", Name = "DeleteArticle")]
    public IActionResult DeleteArticle(string id)
    {
        database.DeleteArticle(id);
        return Ok("Article deleted successfully");
    }

    [HttpPut("{id}", Name = "UpdateArticle")]
    public IActionResult UpdateArticle(string id, [FromBody] Article article)
    {
        database.UpdateArticle(id, article);
        return Ok("Article updated successfully");
    }
    
    [HttpGet("getArticleById/{articleId}", Name = "GetArticleById")]
    public ActionResult<Article> GetArticleById(string articleId)
    {
        var article = database.GetArticleById(articleId);
        if (article is null)
        {
            return NotFound($"Article with ID {articleId} not found.");
        }
        return Ok(article);
    }
}