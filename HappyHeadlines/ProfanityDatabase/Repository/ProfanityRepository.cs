using CoreDomain;
using Microsoft.EntityFrameworkCore;

namespace ProfanityDatabase.Repository;

public class ProfanityRepository(ProfanityContext context) : IProfanityRepository
{
    public async Task<Profanity> CreateProfanity(Profanity profanity)
    {
        context.Profanity.Add(profanity);
        await context.SaveChangesAsync();
        return profanity;
    }

    public async Task<bool> ContainsProfanity(string message)
    {
        var words = await context.Profanity.Select(p => p.Word).ToListAsync();

        return words.Any(w => message.Contains(w, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<Profanity>> GetAllProfanities()
    {
        var profanities = await context.Profanity.ToListAsync();
        return profanities;
    }
}