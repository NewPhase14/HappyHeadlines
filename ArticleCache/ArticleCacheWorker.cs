using ArticleCache.Service;

namespace ArticleCache;

/// <summary>
/// This background service is responsible for refreshing the article cache every 15 minutes.
/// This ensures that the cache remains up-to-date with the latest articles.
/// </summary>
/// <param name="scopeFactory"></param>
public class ArticleCacheWorker(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = scopeFactory.CreateAsyncScope();

            var articleCacheService =
                scope.ServiceProvider
                    .GetRequiredService<IArticleCacheService>();

            await articleCacheService.RefreshCache();
            
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}