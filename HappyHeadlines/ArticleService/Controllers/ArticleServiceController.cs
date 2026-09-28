using ArticleDatabase;
using ArticleService.Interface;
using CoreDomain;
using Microsoft.AspNetCore.Mvc;
using Monitoring;
using Serilog;

namespace ArticleService.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticleServiceController(IArticleService articleService) : ControllerBase
{
    private Database database = Database.GetInstance();
    
    [HttpPost(Name = "CreateArticle")]
    public async Task<IActionResult> CreateArticle([FromBody] Article article)
    {
        
        var success = await articleService.CreateArticle(article);

        if (!success)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "DraftService is currently unavailable.");
        }

        return Ok();
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
}