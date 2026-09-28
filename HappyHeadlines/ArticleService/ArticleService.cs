using System.Diagnostics;
using ArticleDatabase;
using ArticleService.Interface;
using CoreDomain;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace ArticleService;

public class ArticleService(IHttpClientFactory httpClientFactory, TraceContextPropagator traceContextPropagator) : IArticleService
{
    private readonly HttpClient draftClient = httpClientFactory.CreateClient("DraftService");
    private readonly Database database = Database.GetInstance();

    public async Task<bool> CreateArticle(Article article)
    {
      
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

                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"DraftService response: {responseBody}");
            }
            catch (HttpRequestException ex)
            {
                MonitoringService.log.Error("Failed to send draft article to DraftService.", ex);
                Console.WriteLine($"DraftService unavailable: {ex.Message}");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                MonitoringService.log.Error("Failed to send draft article to DraftService.", ex);
                Console.WriteLine($"DraftService timed out: {ex.Message}");
                return false;
            }
        }
        
        MonitoringService.log.Debug("Creating article in database.");

        database.InsertArticle(article);
        return true;
    }
}