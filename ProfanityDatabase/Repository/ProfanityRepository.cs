using CoreDomain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ProfanityDatabase.Repository;

public class ProfanityRepository(ProfanityContext context, IMemoryCache memoryCache) : IProfanityRepository
{
    public async Task<Profanity> CreateProfanity(Profanity profanity)
    {
        context.Profanity.Add(profanity);
        await context.SaveChangesAsync();
        memoryCache.Remove("profanity-words");
        return profanity;
    }

    public async Task<bool> ContainsProfanity(string message)
    {
        var words = await memoryCache.GetOrCreateAsync("profanity-words", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return await context.Profanity.Select(p => p.Word).ToListAsync();
        });
        if (words == null) return false;

        return words.Any(w => message.Contains(w, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<Profanity>> GetAllProfanities()
    {
        var profanities = await context.Profanity.ToListAsync();
        return profanities;
    }
}