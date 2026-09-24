using CoreDomain;
using Npgsql;

namespace DraftDatabase;

public class Database
{
    private static Database instance = new Database();
    
    private readonly NpgsqlConnection connection;
    
    public Database()
    {
        connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("DRAFT_DB_CONNECTION")); 
        connection.Open();
    }

    public static Database GetInstance()
    {
        return instance;
    }
    
    public void DeleteDatabase()
    {
        Execute( $"DROP TABLE IF EXISTS Draft");
    }
    
    public void RecreateDatabase()
    {
        Execute( $"CREATE TABLE Draft(id VARCHAR PRIMARY KEY, title VARCHAR(100), content VARCHAR(1000), continent VARCHAR(50), draft BOOLEAN)");
    }
    
    public void InsertDraft(Article article)
    {
        Execute($"INSERT INTO Draft(id, title, content, continent, draft) VALUES('{article.Id}', '{article.Title}', '{article.Content}', '{article.Continent}', '{article.Draft}')");
    }
    
    public List<Article> GetAllDrafts()
    {
        var drafts = new List<Article>();
        
         var cmd = connection.CreateCommand();
         cmd.CommandText = $"SELECT * FROM Draft";
         using var reader = cmd.ExecuteReader();
         while (reader.Read())
         {
             drafts.Add(new Article
             {
                 Id = reader.GetString(0),
                 Title = reader.GetString(1),
                 Content = reader.GetString(2),
                 Continent = reader.GetString(3),
                 Draft = reader.GetBoolean(4),
             });
         }
         return drafts;
    }
    
    public void UpdateDraft(string id, Article article)
    {
        Execute($"UPDATE Draft SET title = '{article.Title}', content = '{article.Content}', continent = '{article.Continent}', draft = '{article.Draft}' WHERE id = '{id}'");
    }
    
    public void DeleteDraft(string id)
    {
        Execute($"DELETE FROM Draft WHERE id = '{id}'");
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