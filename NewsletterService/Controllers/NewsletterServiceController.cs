using CoreDomain;
using Microsoft.AspNetCore.Mvc;

namespace NewsletterService.Controllers;

[ApiController]
[Route("[controller]")]
public class NewsletterServiceController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    private readonly HttpClient articleClient = httpClientFactory.CreateClient("ArticleService");
    private readonly HttpClient subscriberClient = httpClientFactory.CreateClient("SubscriberService");

    [HttpGet("getDailyNewsletter", Name = "GetDailyNewsletter")]
    public async Task<ActionResult<Article>> GetDailyNewsletter()
    {
        try
        {
            var response = await articleClient.GetAsync(
                "ArticleService/getLatestArticle");

            if (!response.IsSuccessStatusCode)
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Article service is unavailable.");

            var article =
                await response.Content.ReadFromJsonAsync<Article>();

            if (article is null)
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Article service returned an empty response.");

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
    
    [HttpGet("getAllSubscribers", Name = "GetAllSubscribers")]
    public async Task<ActionResult<List<Subscriber>>> GetAllSubscribers()
    {
        try
        {
            var response = await subscriberClient.GetAsync(
                "SubscriberService/getAllSubscribers");

            if (!response.IsSuccessStatusCode)
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Subscriber service is unavailable.");

            var subscribers =
                await response.Content.ReadFromJsonAsync<List<Subscriber>>();

            if (subscribers is null)
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Subscriber service returned an empty response.");

            return Ok(subscribers);
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Subscriber service is unavailable.");
        }
        catch (TaskCanceledException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Subscriber service timed out.");
        }
    }
}