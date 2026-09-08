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
        connection = new NpgsqlConnection($"Server=localhost;Port=5440;Database=comments;User Id=admin;Password=admin;"); 
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
        Execute( $"CREATE TABLE Comment(id VARCHAR PRIMARY KEY, message VARCHAR(1000), author VARCHAR(100))");
    }
    
    
    //Insert an article into a specific continent database
    public void InsertComment(Comment comment)
    {
        Execute($"INSERT INTO Comment(id, message, author) VALUES('{comment.Id}', '{comment.Message}', '{comment.Author}')");
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
                 Message = reader.GetString(1),
                 Author = reader.GetString(2),
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