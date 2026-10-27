using CoreDomain;

namespace SubscriberDatabase.Repository;

public interface ISubscriberRepository
{
    Task<Subscriber> CreateSubscriber(Subscriber subscriber);
    Task DeleteSubscriber(string email);
}