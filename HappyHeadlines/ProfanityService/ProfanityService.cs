using ProfanityDatabase;

namespace ProfanityService;

public class ProfanityService
{
    private readonly Database database = Database.GetInstance();

    public bool ContainsProfanity(string message)
    {
        return database.ContainsProfanity(message);
    }
}