using ArticleDatabase;
using CoreDomain;
using Microsoft.AspNetCore.Mvc;

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
        // TODO: Implement the logic to retrieve a list of articles
        throw new NotImplementedException();
    }

    [HttpDelete("{id}", Name = "DeleteArticle")]
    public IActionResult DeleteArticle(int id)
    {
        // TODO: Implement the logic to delete an article by its ID
        throw new NotImplementedException();
    }

    [HttpPut("{id}", Name = "UpdateArticle")]
    public IActionResult UpdateArticle(int id, [FromBody] Article article)
    {
        // TODO: Implement the logic to update an article by its ID
        throw new NotImplementedException();
    }
}