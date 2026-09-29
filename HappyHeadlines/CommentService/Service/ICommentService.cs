using CoreDomain;

namespace CommentService.Service;

public interface ICommentService
{
    Task CreateComment(Comment comment);
    Task<List<Comment>> GetComments();
    Task<List<Comment>> GetCommentCacheAsync(string articleId);
    Task<List<Comment>> GetCommentsByArticleId(string articleId);
}