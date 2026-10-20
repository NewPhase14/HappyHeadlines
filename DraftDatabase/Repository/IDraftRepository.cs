using CoreDomain;

namespace DraftDatabase.Repository;

public interface IDraftRepository
{
    Task<Article> CreateDraft(Article draft);
    Task<List<Article>> GetAllDrafts();
    Task<Article?> UpdateDraft(Article draft);
    Task DeleteDraft(string draftId);
}