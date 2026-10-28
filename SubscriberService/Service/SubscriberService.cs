using CoreDomain;
using FeatureHubSDK;
using SubscriberDatabase.Repository;
using SubscriberQueue.Interfaces;

namespace SubscriberService.Service;

public class SubscriberService(ISubscriberRepository subscriberRepository, ISubscriberQueue subscriberQueue, IClientContext featureHubContext): ISubscriberService
{
    private const string FeatureFlag = "SUBSCRIBER_SERVICE_ENABLED";
    
    public async Task<Subscriber> CreateSubscriber(Subscriber subscriber)
    {
        if (!featureHubContext.IsEnabled(FeatureFlag))
        {
            throw new InvalidOperationException(
                "Subscriber service is currently disabled."
            );
        }

        var createdSubscriber =
            await subscriberRepository.CreateSubscriber(subscriber);

        await subscriberQueue.PublishAsync(
            "subscriber-created",
            createdSubscriber
        );

        return createdSubscriber;
    }

    public async Task DeleteSubscriber(string email)
    {
        if (!featureHubContext.IsEnabled(FeatureFlag))
        {
            throw new InvalidOperationException(
                "Subscriber service is currently disabled."
            );
        }

        await subscriberRepository.DeleteSubscriber(email);
    }

    public async Task<List<Subscriber>> GetAllSubscribers()
    {
        if (!featureHubContext.IsEnabled(FeatureFlag))
        {
            throw new InvalidOperationException(
                "Subscriber service is currently disabled."
            );
        }
        
        var subscribers = await subscriberRepository.GetAllSubscribers();
        return subscribers;
    }
}