using System.Net;
using CoreDomain;
using Microsoft.AspNetCore.Mvc;

namespace NewsletterService.Controllers;

[ApiController]
[Route("[controller]")]
public class NewsletterServiceController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    private readonly HttpClient articleClient = httpClientFactory.CreateClient("ArticleService");

    [HttpGet(Name = "GetDailyNewsletter")]
    public async Task<ActionResult<Article>> GetDailyNewsletter()
    {
        try
        {
            var response = await articleClient.GetAsync(
                "ArticleService/getRandomArticle");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Article service is unavailable.");
            }

            var article =
                await response.Content.ReadFromJsonAsync<Article>();

            if (article is null)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Article service returned an empty response.");
            }

            return Ok(article);
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Article service is unavailable.");
        }
        catch (TaskCanceledException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Article service timed out.");
        }
    }
}