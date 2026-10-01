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
    IArticlePublisher articlePublisher,
    IHttpClientFactory httpClientFactory,
    TraceContextPropagator traceContextPropagator) : ControllerBase
{
    private readonly HttpClient draftClient = httpClientFactory.CreateClient("DraftService");

    [HttpPost(Name = "PublishArticle")]
    public async Task<IActionResult> PublishArticle([FromBody] CreateArticleDto createArticleDto)
    {
        //TODO: Error: Getting double messages on newsletter service. On article service they are split.

        using var activity = MonitoringService.ActivitySource.StartActivity();

        if (createArticleDto.Draft)
        {
            MonitoringService.log.Debug(
                "Sending draft article to DraftService for creation.");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "DraftService/createDraft");

            request.Content = JsonContent.Create(createArticleDto);

            var activityContext =
                activity?.Context ??
                Activity.Current?.Context ??
                default;

            var propagationContext =
                new PropagationContext(activityContext, Baggage.Current);

            traceContextPropagator.Inject(
                propagationContext,
                request,
                (request, key, value) => { request.Headers.TryAddWithoutValidation(key, value); });

            var response = await draftClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            return Ok();
        }

        await articlePublisher.PublishArticle(createArticleDto);

        MonitoringService.log.Debug("Creating article in database.");

        return Ok();
    }
}