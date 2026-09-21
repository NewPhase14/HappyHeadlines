using System.Data.Common;
using CoreDomain;
using Npgsql;


namespace ArticleDatabase;

public class Coordinator
{
    private IDictionary<string, DbConnection> ConnectionCache = new Dictionary<string, DbConnection>();
    
    // Database for each continent + global database
    private DatabaseStruct ASIA_DB = new DatabaseStruct { name = "ASIA"};
    private DatabaseStruct EUROPE_DB = new DatabaseStruct { name = "EUROPE" };
    private DatabaseStruct NORTH_AMERICA_DB = new DatabaseStruct { name = "NORTH_AMERICA" };
    private DatabaseStruct SOUTH_AMERICA_DB = new DatabaseStruct { name = "SOUTH_AMERICA" };
    private DatabaseStruct AFRICA_DB = new DatabaseStruct { name = "AFRICA"};
    private DatabaseStruct OCEANIA_DB = new DatabaseStruct { name = "OCEANIA" };
    private DatabaseStruct ANTARCTICA_DB = new DatabaseStruct { name = "ANTARCTICA" };
    private DatabaseStruct GLOBAL_DB = new DatabaseStruct { name = "GLOBAL"};
    
    public List<DbConnection> GetAllConnections()
    {
        List<DbConnection> connections = new List<DbConnection>();

        connections.Add(new NpgsqlConnection(Environment.GetEnvironmentVariable("ASIA_DB_CONNECTION")));
        connections.Add(new NpgsqlConnection(Environment.GetEnvironmentVariable("EUROPE_DB_CONNECTION")));
        connections.Add(new NpgsqlConnection(Environment.GetEnvironmentVariable("NORTH_AMERICA_DB_CONNECTION")));
        connections.Add(new NpgsqlConnection(Environment.GetEnvironmentVariable("SOUTH_AMERICA_DB_CONNECTION")));
        connections.Add(new NpgsqlConnection(Environment.GetEnvironmentVariable("AFRICA_DB_CONNECTION")));
        connections.Add(new NpgsqlConnection(Environment.GetEnvironmentVariable("OCEANIA_DB_CONNECTION")));
        connections.Add(new NpgsqlConnection(Environment.GetEnvironmentVariable("ANTARCTICA_DB_CONNECTION")));
        connections.Add(new NpgsqlConnection(Environment.GetEnvironmentVariable("GLOBAL_DB_CONNECTION")));

        foreach (var connection in connections)
        {
            connection.Open();
        }
        return connections;
    }

    public DbConnection GetConnectionByServerName(string serverName)
    {
        var databaseStruct = serverName switch
        {
            "Asia" => ASIA_DB,
            "Europe" => EUROPE_DB,
            "North America" => NORTH_AMERICA_DB,
            "South America" => SOUTH_AMERICA_DB,   
            "Africa" => AFRICA_DB,
            "Oceania" => OCEANIA_DB,
            "Antarctica" => ANTARCTICA_DB,
            _ => GLOBAL_DB
        };
        
        if (ConnectionCache.TryGetValue(serverName, out var connection))
        {
            return connection;
        }
        
        connection = new NpgsqlConnection(Environment.GetEnvironmentVariable($"{databaseStruct.name}_DB_CONNECTION"));
        connection.Open();
        ConnectionCache.Add(serverName, connection);
        return connection;
    }
}