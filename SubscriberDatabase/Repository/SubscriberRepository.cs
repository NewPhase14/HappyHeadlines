using CoreDomain;
using Microsoft.EntityFrameworkCore;

namespace SubscriberDatabase.Repository;

public class SubscriberRepository(SubscriberContext context) : ISubscriberRepository 
{
    public async Task<Subscriber> CreateSubscriber(Subscriber subscriber)
    {
        await context.Subscribers.AddAsync(subscriber);
        await context.SaveChangesAsync();
        return subscriber;
    }

    public async Task DeleteSubscriber(string email)
    {
        var subscriber = await context.Subscribers.FirstOrDefaultAsync(sub => sub.Email == email);
        
        if (subscriber != null)
        {
            context.Subscribers.Remove(subscriber);
            await context.SaveChangesAsync();
        }
    }
}