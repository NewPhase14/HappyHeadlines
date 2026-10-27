using CoreDomain;

namespace SubscriberService.Service;

public interface ISubscriberService
{
    Task<Subscriber> CreateSubscriber(Subscriber subscriber);
    Task DeleteSubscriber(string email);
}