using System.Data.Common;
using Microsoft.Data.SqlClient;


namespace ArticleDatabase;

public class Coordinator
{
    private IDictionary<string, DbConnection> ConnectionCache = new Dictionary<string, DbConnection>();

    // Database for each continent
    private const string ASIA_DB = "asia-db";
    private const string EUROPE_DB = "europe-db";
    private const string NORTH_AMERICA_DB = "north-america-db";
    private const string SOUTH_AMERICA_DB = "south-america-db";
    private const string AFRICA_DB = "africa-db";
    private const string OCEANIA_DB = "oceania-db";
    private const string ANTARCTICA_DB = "antarctica-db";
    private const string GLOBAL_DB = "global-db";
    
    public DbConnection GetAsiaConnection()
    {
        return GetConnectionByServerName(ASIA_DB);
    }   
    
    public DbConnection GetEuropeConnection()
    {
        return GetConnectionByServerName(EUROPE_DB);
    }
    
    public DbConnection GetNorthAmericaConnection()
    {
        return GetConnectionByServerName(NORTH_AMERICA_DB);
    }
    
    public DbConnection GetSouthAmericaConnection()
    {
        return GetConnectionByServerName(SOUTH_AMERICA_DB);
    }
    
    public DbConnection GetAfricaConnection()
    {
        return GetConnectionByServerName(AFRICA_DB);
    }
    
    public DbConnection GetOceaniaConnection()
    {
        return GetConnectionByServerName(OCEANIA_DB);
    }
    
    public DbConnection GetAntarcticaConnection()
    {
        return GetConnectionByServerName(ANTARCTICA_DB);
    }
    
    public DbConnection GetGlobalConnection()
    {
        return GetConnectionByServerName(GLOBAL_DB);
    }

    
    public DbConnection GetConnectionByServerName(string serverName)
    {
        if (ConnectionCache.TryGetValue(serverName, out var connection))
        {
            return connection;
        }
        
        connection = new SqlConnection($"Server={serverName};User Id=sa;Password=SuperSecret7!;Encrypt=false;");
        connection.Open();
        ConnectionCache.Add(serverName, connection);
        return connection;
    }
    
}