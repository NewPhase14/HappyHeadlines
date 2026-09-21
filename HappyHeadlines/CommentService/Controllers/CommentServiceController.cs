using CommentDatabase;
using CoreDomain;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentServiceController : ControllerBase
{
    private Database database = Database.GetInstance();
    
    [HttpPost(Name = "CreateComment")]
    public async Task<IActionResult> CreateComment([FromBody] Comment comment)
    {
        HttpClient profanityClient = new HttpClient();
        var response = await profanityClient.PostAsJsonAsync(
            "http://profanity-service:8080/ProfanityService/checkProfanity",
            comment.Message
        ); 
        try 
        {
            response.EnsureSuccessStatusCode();
            
                bool containsProfanity = await response.Content.ReadFromJsonAsync<bool>();
                if (containsProfanity)
                {
                    return BadRequest("Comment contains profanity and cannot be added.");
                }
            
                database.InsertComment(comment);
                return Ok();
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while checking for profanity: " + ex.Message);
        }
    }

    [HttpGet(Name = "GetComments")]
    public ActionResult<List<Comment>>GetComments()
    {
        return Ok(database.GetAllComments());
    }

    
}