using CoreDomain;

namespace CommentCache.Service;

public interface ICommentCacheService
{
    Task<List<Comment>> GetCommentsByArticleId(string articleId);
}