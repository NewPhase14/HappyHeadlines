using System.Data.Common;
using CoreDomain;
using Npgsql;

namespace CommentDatabase;

public class Database
{
    
    private static Database instance = new Database();
    
    private readonly NpgsqlConnection connection;
    
    public Database()
    {
        connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("COMMENT_DB_CONNECTION")); 
        connection.Open();
    }

    public static Database GetInstance()
    {
        return instance;
    }
    
    public void DeleteDatabase()
    {
        Execute( $"DROP TABLE IF EXISTS Comment");
        
    }
    
    public void RecreateDatabase()
    {
        Execute( $"CREATE TABLE Comment(id VARCHAR PRIMARY KEY, article_id VARCHAR, message VARCHAR(1000), author VARCHAR(100), created_at TIMESTAMP WITH TIME ZONE)");
    }
    
    
    //Insert an article into a specific continent database
    public void InsertComment(Comment comment)
    {
        Execute($"INSERT INTO Comment(id, article_id, message, author, created_at) VALUES('{comment.Id}', '{comment.ArticleId}', '{comment.Message}', '{comment.Author}', '{DateTime.UtcNow}')");
    }
    
    //Get all comments
    public List<Comment> GetAllComments()
    {
        var comments = new List<Comment>();
        
         var cmd = connection.CreateCommand();
         cmd.CommandText = $"SELECT * FROM Comment";
         using var reader = cmd.ExecuteReader();
         while (reader.Read())
         {
             comments.Add(new Comment
             {
                 Id = reader.GetString(0),
                 ArticleId = reader.GetString(1),
                 Message = reader.GetString(2),
                 Author = reader.GetString(3),
                 CreatedAt = reader.GetDateTime(4),
             });
         }
        return comments;
    }
    
    //Get comments by article id
    public List<Comment> GetCommentsByArticleId(string articleId)
    {
        var comments = new List<Comment>();
        
         var cmd = connection.CreateCommand();
         cmd.CommandText = $"SELECT * FROM Comment WHERE article_id = '{articleId}'";
         using var reader = cmd.ExecuteReader();
         while (reader.Read())
         {
             comments.Add(new Comment
             {
                 Id = reader.GetString(0),
                 ArticleId = reader.GetString(1),
                 Message = reader.GetString(2),
                 Author = reader.GetString(3),
                 CreatedAt = reader.GetDateTime(4),
             });
         }
        return comments;
    }
    
    private void Execute(string sql)
    {
        using var trans = connection.BeginTransaction();
        var cmd = connection.CreateCommand();
        cmd.Transaction = trans;
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
        trans.Commit();
    }
}