using ArticleDatabase;
using CoreDomain;
using Microsoft.AspNetCore.Mvc;
using Monitoring;
using Serilog;

namespace ArticleService.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticleController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    private Database database = Database.GetInstance();
    private readonly HttpClient draftClient = httpClientFactory.CreateClient("DraftService");

    
    [HttpPost(Name = "CreateArticle")]
    public async Task<IActionResult> CreateArticle([FromBody] Article article)
    {
        // Only contact DraftService if this is a draft
        if (article.Draft)
        {
            try
            {
                var response = await draftClient.PostAsJsonAsync(
                    "DraftService/createDraft",
                    article);
                
                Console.WriteLine(
                    $"DraftService response: {(int)response.StatusCode} " +
                    $"{response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(
                        StatusCodes.Status503ServiceUnavailable,
                        "Draft service is unavailable.");
                }
                
                // Draft was saved by DraftService.
                // Do NOT save it in ArticleDatabase.
                return Ok("Draft created successfully.");
                
            }
            catch (HttpRequestException)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Draft service is unavailable.");
            }
            catch (TaskCanceledException)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Draft service timed out.");
            }
        }

        try
        {
            database.InsertArticle(article);
            return Ok();
        }
        catch
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Article database is unavailable.");
        }
        
    }

    [HttpGet(Name = "GetArticles")]
    public ActionResult<List<Article>>GetArticles()
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        
        Console.WriteLine($"Activity is null: {activity is null}");
        
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