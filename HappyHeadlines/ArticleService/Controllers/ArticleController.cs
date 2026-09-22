using ArticleDatabase;
using CoreDomain;
using Microsoft.AspNetCore.Mvc;
using Monitoring;
using Serilog;

namespace ArticleService.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticleController : ControllerBase
{
    private Database database = Database.GetInstance();
    
    [HttpPost(Name = "CreateArticle")]
    public IActionResult CreateArticle([FromBody] Article article)
    {
        database.InsertArticle(article);
        return Ok();
    }

    [HttpGet(Name = "GetArticles")]
    public ActionResult<List<Article>>GetArticles()
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        
        Console.WriteLine($"Activity is null: {activity is null}");
        
        MonitoringService.log.Information("GetArticles endpoint called - v1");

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
}