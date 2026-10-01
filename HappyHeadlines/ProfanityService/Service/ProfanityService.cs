using CoreDomain;
using CoreDomain.Dtos.ProfanityDtos;
using ProfanityDatabase.Repository;

namespace ProfanityService.Service;

public class ProfanityService(IProfanityRepository profanityRepository) : IProfanityService
{
    public async Task<ProfanityResponseDto> CreateProfanity(CreateProfanityDto createProfanityDto)
    {
        var profanity = new Profanity
        {
            Id = Guid.NewGuid().ToString(),
            Word = createProfanityDto.Word
        };

        var createdProfanity = await profanityRepository.CreateProfanity(profanity);

        return new ProfanityResponseDto
        {
            Id = createdProfanity.Id,
            Word = createdProfanity.Word
        };
    }

    public async Task<bool> ContainsProfanity(string message)
    {
        return await profanityRepository.ContainsProfanity(message);
    }

    public async Task<List<ProfanityResponseDto>> GetAllProfanities()
    {
        var profanities = await profanityRepository.GetAllProfanities();

        return profanities.Select(p => new ProfanityResponseDto
        {
            Id = p.Id,
            Word = p.Word
        }).ToList();
    }
}