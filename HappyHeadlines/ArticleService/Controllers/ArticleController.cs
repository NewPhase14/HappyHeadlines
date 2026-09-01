using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticleController : ControllerBase
{
    [HttpPost(Name = "CreateArticle")]
    public IActionResult CreateArticle([FromBody] Article article)
    {
        // TODO: Implement the logic to create a new article
        throw new NotImplementedException();
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