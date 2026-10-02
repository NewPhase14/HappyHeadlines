using CoreDomain.Dtos.ProfanityDtos;

namespace ProfanityService.Service;

public interface IProfanityService
{
    Task<ProfanityResponseDto> CreateProfanity(CreateProfanityDto createProfanityDto);
    Task<bool> ContainsProfanity(string message);
    Task<List<ProfanityResponseDto>> GetAllProfanities();
}