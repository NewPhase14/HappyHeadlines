using CoreDomain;
using Microsoft.EntityFrameworkCore;

namespace DraftDatabase.Repository;

public class DraftRepository(DraftContext context) : IDraftRepository
{
    public async Task<Article> CreateDraft(Article draft)
    {
        context.Drafts.Add(draft);
        await context.SaveChangesAsync();
        return draft;
    }

    public async Task<List<Article>> GetAllDrafts()
    {
        var drafts = await context.Drafts.ToListAsync();
        return drafts;
    }

    public async Task<Article?> UpdateDraft(Article draft)
    {
        var existingDraft = await context.Drafts.FirstOrDefaultAsync(d => d.Id == draft.Id);
        if (existingDraft != null)
        {
            existingDraft.Title = draft.Title;
            existingDraft.Content = draft.Content;
            existingDraft.Continent = draft.Continent;
            existingDraft.Draft = draft.Draft;

            await context.SaveChangesAsync();

            return existingDraft;
        }

        return null;
    }

    public async Task DeleteDraft(string draftId)
    {
        var draft = await context.Drafts.FirstOrDefaultAsync(d => d.Id == draftId);
        if (draft != null)
        {
            context.Drafts.Remove(draft);
            await context.SaveChangesAsync();
        }
    }
}