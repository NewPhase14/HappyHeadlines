using CoreDomain;

namespace ProfanityDatabase.Repository;

public interface IProfanityRepository
{
    Task<Profanity> CreateProfanity(Profanity profanity);

    Task<bool> ContainsProfanity(string message);

    Task<List<Profanity>> GetAllProfanities();
}