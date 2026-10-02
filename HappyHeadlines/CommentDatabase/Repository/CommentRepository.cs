using CoreDomain;
using Microsoft.EntityFrameworkCore;

namespace CommentDatabase.Repository;

public class CommentRepository(CommentContext context) : ICommentRepository
{
    public async Task<Comment> CreateComment(Comment comment)
    {
        context.Comments.Add(comment);
        await context.SaveChangesAsync();
        return comment;
    }

    public async Task<List<Comment>> GetAllComments()
    {
        var comments = await context.Comments.ToListAsync();
        return comments;
    }

    public async Task<List<Comment>?> GetCommentsByArticleId(string articleId)
    {
        var comments = await context.Comments
            .Where(c => c.ArticleId == articleId)
            .ToListAsync();
        return comments;
    }
}