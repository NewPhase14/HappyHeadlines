using CoreDomain.Dtos.CommentDtos;

namespace CommentService.Service;

public interface ICommentService
{
    Task<CommentResponseDto> CreateComment(CreateCommentDto createCommentDto);
    Task<List<CommentResponseDto>> GetAllComments();
    Task<List<CommentResponseDto>> GetCommentCacheAsync(string articleId);
    Task<List<CommentResponseDto>> GetCommentsByArticleId(string articleId);
}