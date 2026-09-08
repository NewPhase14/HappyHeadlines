using System.Data.Common;
using Microsoft.Data.SqlClient;

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
        Execute(coordinator.GetAfricaConnection(), "DROP TABLE IF EXISTS AfricaDB");
        Execute(coordinator.GetAntarcticaConnection(), "DROP TABLE IF EXISTS AntarcticaDB");
        Execute(coordinator.GetAsiaConnection(), "DROP TABLE IF EXISTS AsiaDB");
        Execute(coordinator.GetEuropeConnection(), "DROP TABLE IF EXISTS EuropeDB");
        Execute(coordinator.GetNorthAmericaConnection(), "DROP TABLE IF EXISTS NorthAmericaDB");
        Execute(coordinator.GetSouthAmericaConnection(), "DROP TABLE IF EXISTS SouthAmericaDB");
        Execute(coordinator.GetOceaniaConnection(), "DROP TABLE IF EXISTS OceaniaDB");
        Execute(coordinator.GetGlobalConnection(), "DROP TABLE IF EXISTS GlobalDB");
    }
    
    public void RecreateDatabase()
    {
        Execute(coordinator.GetAfricaConnection(), "CREATE TABLE AfricaDB(id varchar PRIMARY KEY, title VARCHAR(100), content VARCHAR(max), continent VARCHAR(50))");
        Execute(coordinator.GetAntarcticaConnection(), "CREATE TABLE AntarcticaDB(id varchar PRIMARY KEY, title VARCHAR(100), content VARCHAR(max), continent VARCHAR(50))");
        Execute(coordinator.GetAsiaConnection(), "CREATE TABLE AsiaDB(id varchar PRIMARY KEY, title VARCHAR(100), content VARCHAR(max), continent VARCHAR(50))");
        Execute(coordinator.GetEuropeConnection(), "CREATE TABLE EuropeDB(id varchar PRIMARY KEY, title VARCHAR(100), content VARCHAR(max), continent VARCHAR(50))");
        Execute(coordinator.GetNorthAmericaConnection(), "CREATE TABLE NorthAmericaDB(id varchar PRIMARY KEY, title VARCHAR(100), content VARCHAR(max), continent VARCHAR(50))");
        Execute(coordinator.GetSouthAmericaConnection(), "CREATE TABLE SouthAmericaDB(id varchar PRIMARY KEY, title VARCHAR(100), content VARCHAR(max), continent VARCHAR(50))");
        Execute(coordinator.GetOceaniaConnection(), "CREATE TABLE OceaniaDB(id varchar PRIMARY KEY, title VARCHAR(100), content VARCHAR(max), continent VARCHAR(50))");
        Execute(coordinator.GetGlobalConnection(), "CREATE TABLE GlobalDB(id varchar PRIMARY KEY, title VARCHAR(100), content VARCHAR(max), continent VARCHAR(50))");
    }
    
    //Get all articles from a specific continent database
    
    //Insert an article into a specific continent database
    public void InsertArticle(string continent, string id, string title, string content)
    {
        var connection = coordinator.GetConnectionByServerName(continent);
        Execute(connection, $"INSERT INTO {continent}DB(id, title, content, continent) VALUES('{id}', '{title}', '{content}', '{continent}')");
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