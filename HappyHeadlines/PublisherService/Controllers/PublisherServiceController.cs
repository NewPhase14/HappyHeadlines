using System.Diagnostics;
using ArticleQueue.Interfaces;
using CoreDomain;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using PublisherService.Infrastructure;

namespace PublisherService.Controllers;

[ApiController]
[Route("[controller]")]
public class PublisherServiceController(IArticlePublisher articlePublisher, IHttpClientFactory httpClientFactory, TraceContextPropagator traceContextPropagator) : ControllerBase
{
    private readonly HttpClient draftClient = httpClientFactory.CreateClient("DraftService");
    
    [HttpPost(Name = "PublishArticle")]
    public async Task<IActionResult> PublishArticle([FromBody] Article article)
    {
        //TODO: Error: Getting double messages on newsletter service. On article service they are split.
        
        
       using var activity =
            MonitoringService.ActivitySource.StartActivity();

        if (article.Draft)
        {
            try
            {
                MonitoringService.log.Debug("Sending draft article to DraftService for creation.");
                
                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "DraftService/createDraft");

                request.Content = JsonContent.Create(article);

                var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
                var propagationContext = new PropagationContext(activityContext, Baggage.Current);
                
                // Inject current Activity into HTTP headers
                traceContextPropagator.Inject(
                    propagationContext,
                    request,
                    (request, key, value) =>
                    {
                        request.Headers.Add(key, value);
                    });

                var response = await draftClient.SendAsync(request);

                Console.WriteLine($"DraftService status: {response.StatusCode}");

                var responseBody = response.Content.ReadAsStringAsync();
                Console.WriteLine($"DraftService response: {responseBody}");
            }
            catch (HttpRequestException ex)
            {
                MonitoringService.log.Error("Failed to send draft article to DraftService.", ex);
                Console.WriteLine($"DraftService unavailable: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                MonitoringService.log.Error("Failed to send draft article to DraftService.", ex);
                Console.WriteLine($"DraftService timed out: {ex.Message}");
            }
        }
        else
        {
            await articlePublisher.PublishArticle(article);
            MonitoringService.log.Debug("Creating article in database.");
            return Ok();
        }
        return BadRequest();
    }
    
    
}