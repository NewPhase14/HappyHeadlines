using CoreDomain;
using CoreDomain.Dtos.ArticleDtos;
using DraftDatabase.Repository;

namespace DraftService.Service;

public class DraftService(IDraftRepository repository) : IDraftService
{
    public async Task<ArticleResponseDto> CreateDraft(CreateArticleDto createArticleDto)
    {
        var draft = new Article
        {
            Id = Guid.NewGuid().ToString(),
            Title = createArticleDto.Title,
            Content = createArticleDto.Content,
            Continent = createArticleDto.Continent,
            Draft = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdDraft = await repository.CreateDraft(draft);

        return new ArticleResponseDto
        {
            Id = createdDraft.Id,
            Title = createdDraft.Title,
            Content = createdDraft.Content,
            Continent = createdDraft.Continent,
            Draft = createdDraft.Draft,
            CreatedAt = createdDraft.CreatedAt
        };
    }

    public async Task<List<ArticleResponseDto>> GetAllDrafts()
    {
        var drafts = await repository.GetAllDrafts();

        return drafts.Select(d => new ArticleResponseDto
        {
            Id = d.Id,
            Title = d.Title,
            Content = d.Content,
            Continent = d.Continent,
            Draft = d.Draft,
            CreatedAt = d.CreatedAt
        }).ToList();
    }

    public async Task<ArticleResponseDto> UpdateDraft(UpdateArticleDto updateArticleDto)
    {
        var draftToBeUpdated = new Article
        {
            Id = updateArticleDto.Id,
            Title = updateArticleDto.Title,
            Content = updateArticleDto.Content,
            Continent = updateArticleDto.Continent,
            Draft = updateArticleDto.Draft
        };
        var updatedDraft = await repository.UpdateDraft(draftToBeUpdated);

        if (updatedDraft == null) throw new ArgumentException("No draft found with the given ID.");

        return new ArticleResponseDto
        {
            Id = updatedDraft.Id,
            Title = updatedDraft.Title,
            Content = updatedDraft.Content,
            Continent = updatedDraft.Continent,
            Draft = updatedDraft.Draft,
            CreatedAt = updatedDraft.CreatedAt
        };
    }

    public async Task DeleteDraft(string draftId)
    {
        await repository.DeleteDraft(draftId);
    }
}