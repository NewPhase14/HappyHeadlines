using CoreDomain.Dtos.ProfanityDtos;
using Microsoft.AspNetCore.Mvc;
using ProfanityService.Service;

namespace ProfanityService.Controllers;

[ApiController]
[Route("[controller]")]
public class ProfanityServiceController(IProfanityService profanityService) : ControllerBase
{
    [HttpPost("checkProfanity")]
    public async Task<ActionResult<bool>> CheckProfanity([FromBody] string message)
    {
        var containsProfanity = await profanityService.ContainsProfanity(message);

        return Ok(containsProfanity);
    }

    [HttpPost(Name = "CreateWord")]
    public async Task<ActionResult<ProfanityResponseDto>> CreateProfanity(
        [FromBody] CreateProfanityDto createProfanityDto)
    {
        var word = await profanityService.CreateProfanity(createProfanityDto);

        return Ok(word);
    }

    [HttpGet(Name = "GetAllProfanities")]
    public async Task<ActionResult<List<ProfanityResponseDto>>> GetAllProfanities()
    {
        var profanities = await profanityService.GetAllProfanities();

        return Ok(profanities);
    }
}