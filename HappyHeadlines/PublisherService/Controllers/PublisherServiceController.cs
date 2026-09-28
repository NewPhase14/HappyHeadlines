using ArticleQueue.Interfaces;
using CoreDomain;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using PublisherService.Infrastructure;

namespace PublisherService.Controllers;

[ApiController]
[Route("[controller]")]
public class PublisherServiceController(IArticlePublisher articlePublisher) : ControllerBase
{

    [HttpPost(Name = "PublishArticle")]
    public IActionResult PublishArticle([FromBody] Article article)
    {
        if (!article.Draft)
        {
            articlePublisher.PublishArticle(article);
            
            Console.WriteLine(
                $"Published article with ID: {article.Id} and Title: {article.Title}");
        }
        return Ok();
    }
    
    
}