using CoreDomain.Dtos.ArticleDtos;

namespace DraftService.Service;

public interface IDraftService
{
    Task<ArticleResponseDto> CreateDraft(CreateArticleDto createArticleDto);
    Task<List<ArticleResponseDto>> GetAllDrafts();
    Task<ArticleResponseDto> UpdateDraft(UpdateArticleDto updateArticleDto);
    Task DeleteDraft(string draftId);
}