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
            Execute(connection, $"CREATE TABLE Article(id VARCHAR PRIMARY KEY, title VARCHAR(100), content VARCHAR(1000), continent VARCHAR(50), created_at TIMESTAMP WITH TIME ZONE, draft BOOLEAN)");
        }
    }
    
    //Insert an article into a specific continent database
    public void InsertArticle(Article article)
    {
        var connection = coordinator.GetConnectionByServerName(article.Continent);
        Execute(connection, $"INSERT INTO Article(id, title, content, continent, created_at, draft) VALUES('{article.Id}', '{article.Title}', '{article.Content}', '{article.Continent}', '{DateTime.Now}', '{article.Draft}')");
    }
    
    //Get all article
    public List<Article> GetAllArticles()
    {
        var articles = new List<Article>();
        var connections = coordinator.GetAllConnections();
        foreach (var connection in connections)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM Article";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                articles.Add(new Article
                {
                    Id = reader.GetString(0),
                    Title = reader.GetString(1),
                    Content = reader.GetString(2),
                    Continent = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4),
                    Draft = reader.GetBoolean(5)
                });
            }
        }
        return articles;
    }
    
    //Delete an article by its ID
    public void DeleteArticle(string id)
    {
        var connections = coordinator.GetAllConnections();
        foreach (var connection in connections)
        {
            Execute(connection, $"DELETE FROM Article WHERE id = '{id}'");
        }
    }
    
    //Update an article by its ID
    public void UpdateArticle(string id, Article article)
    {
        var connections = coordinator.GetAllConnections();
        foreach (var connection in connections)
        {
            Execute(connection, $"UPDATE Article SET title = '{article.Title}', content = '{article.Content}', continent = '{article.Continent}', draft = '{article.Draft}' WHERE id = '{id}'");
        }
    }
    
    //Get latest articles from all databases last 14 days
    public List<Article> GetLatestArticles()
    {
        var articles = new List<Article>();
        var connections = coordinator.GetAllConnections();
        foreach (var connection in connections)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Article WHERE created_at >= NOW() - INTERVAL '14 days'";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                articles.Add(new Article
                {
                    Id = reader.GetString(0),
                    Title = reader.GetString(1),
                    Content = reader.GetString(2),
                    Continent = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4),
                    Draft = reader.GetBoolean(5)
                });
            }
        }
        return articles;
    }
    
    //Get an article by its ID
    public Article? GetArticleById(string id)
    {
        var connections = coordinator.GetAllConnections();
        foreach (var connection in connections)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM Article WHERE id = '{id}'";
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Article
                {
                    Id = reader.GetString(0),
                    Title = reader.GetString(1),
                    Content = reader.GetString(2),
                    Continent = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4),
                    Draft = reader.GetBoolean(5)
                };
            }
        }
        return null;
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