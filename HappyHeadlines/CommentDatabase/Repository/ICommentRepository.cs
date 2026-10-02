using CoreDomain;

namespace CommentDatabase.Repository;

public interface ICommentRepository
{
    Task<Comment> CreateComment(Comment comment);
    Task<List<Comment>> GetAllComments();
    Task<List<Comment>?> GetCommentsByArticleId(string articleId);
}