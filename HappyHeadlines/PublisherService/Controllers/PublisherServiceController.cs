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
        MonitoringService.ActivitySource.StartActivity();

        await articlePublisher.PublishArticle(createArticleDto);

        MonitoringService.log.Debug("Creating article in database.");

        return Ok();
    }
}