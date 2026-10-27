using CoreDomain;
using SubscriberDatabase.Repository;
using SubscriberQueue.Interfaces;

namespace SubscriberService.Service;

public class SubscriberService(ISubscriberRepository subscriberRepository, ISubscriberQueue subscriberQueue): ISubscriberService
{
    public async Task<Subscriber> CreateSubscriber(Subscriber subscriber)
    {
        var createdSubscriber = await subscriberRepository.CreateSubscriber(subscriber);

        await subscriberQueue.PublishAsync("subscriber-created", createdSubscriber);
        
        return createdSubscriber;
    }

    public async Task DeleteSubscriber(string email)
    {
        await subscriberRepository.DeleteSubscriber(email);
    }
}