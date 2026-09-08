using System.Data.Common;
using CoreDomain;

namespace ArticleDatabase;

public class Database
{
    private Coordinator coordinator = new Coordinator();
    private static Database instance = new Database();

    public static Database GetInstance()
    {
        return instance;
    }

    public void DeleteDatabase()
    {
        var connections = coordinator.GetAllConnections();
        
        foreach (var connection in connections)
        {
            Execute(connection, $"DROP TABLE IF EXISTS Article");
        }
    }
    
    public void RecreateDatabase()
    {
        var connections = coordinator.GetAllConnections();

        foreach (var connection in connections)
        { 
            Execute(connection, $"CREATE TABLE Article(id VARCHAR PRIMARY KEY, title VARCHAR(100), content VARCHAR(1000), continent VARCHAR(50))");
        }
    }
    
    //Get all articles from a specific continent database
    
    //Insert an article into a specific continent database
    public void InsertArticle(Article article)
    {
        var connection = coordinator.GetConnectionByServerName(article.Continent);
        Execute(connection, $"INSERT INTO Article(id, title, content, continent) VALUES('{article.Id}', '{article.Title}', '{article.Content}', '{article.Continent}')");
    }

    private void Execute(DbConnection connection, string sql)
    {
        using var trans = connection.BeginTransaction();
        var cmd = connection.CreateCommand();
        cmd.Transaction = trans;
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
        trans.Commit();
    }
}