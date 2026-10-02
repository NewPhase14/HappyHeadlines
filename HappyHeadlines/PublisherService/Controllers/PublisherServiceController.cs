using System.Diagnostics;
using CoreDomain.Dtos.ArticleDtos;
using Microsoft.AspNetCore.Mvc;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using PublisherService.Infrastructure;

namespace PublisherService.Controllers;

[ApiController]
[Route("[controller]")]
public class PublisherServiceController(
    IArticlePublisher articlePublisher) : ControllerBase
{

    [HttpPost(Name = "PublishArticle")]
    public async Task<IActionResult> PublishArticle([FromBody] CreateArticleDto createArticleDto)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        
        MonitoringService.log.Information("Publishing article with title: " + createArticleDto.Title);

        await articlePublisher.PublishArticle(createArticleDto);
        
        MonitoringService.log.Information("Article published");

        return Ok();
    }
}