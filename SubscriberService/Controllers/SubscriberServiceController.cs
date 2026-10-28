using CoreDomain;
using Microsoft.AspNetCore.Mvc;
using SubscriberService.Service;

namespace SubscriberService.Controllers;

[ApiController]
[Route("[controller]")]
public class SubscriberServiceController(ISubscriberService subscriberService) : ControllerBase
{
    [HttpPost(Name = "CreateSubscriber")]
    public async Task<ActionResult<Subscriber>> CreateSubscriber([FromBody] Subscriber subscriber)
    {
        var createdSubscriber = await subscriberService.CreateSubscriber(subscriber);
        return Ok(createdSubscriber);
    }
    
    [HttpDelete("deleteSubscriber/{email}",Name = "DeleteSubscriber")]
    public async Task<IActionResult> DeleteSubscriber(string email)
    { 
        await subscriberService.DeleteSubscriber(email);
        return Ok("Subscriber deleted successfully.");
    }
    
    [HttpGet("getAllSubscribers",Name = "GetAllSubscribers")]
    public async Task<ActionResult<List<Subscriber>>> GetAllSubscribers()
    {
        var subscribers = await subscriberService.GetAllSubscribers();
        return Ok(subscribers);
    }

   
}