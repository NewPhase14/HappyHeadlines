using CommentDatabase;
using CoreDomain;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentServiceController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    private readonly Database database = Database.GetInstance();
    private readonly HttpClient profanityClient = httpClientFactory.CreateClient("ProfanityService");

    [HttpPost(Name = "CreateComment")]
    public async Task<IActionResult> CreateComment([FromBody] Comment comment)
    {
        try
        {
            var response = await profanityClient.PostAsJsonAsync(
                "ProfanityService/checkProfanity",
                comment.Message);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Profanity service is unavailable.");
            }

            var containsProfanity =
                await response.Content.ReadFromJsonAsync<bool>();

            if (containsProfanity)
            {
                return BadRequest(
                    "Comment contains profanity and cannot be added.");
            }
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Profanity service is unavailable.");
        }
        try
        {
            database.InsertComment(comment);
            return Ok();
        }
        catch
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Comment database is unavailable.");
        }
    }

    [HttpGet(Name = "GetComments")]
    public ActionResult<List<Comment>> GetComments()
    {
        try
        {
            return Ok(database.GetAllComments());
        }
        catch
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Comment database is unavailable.");
        }
    }
}