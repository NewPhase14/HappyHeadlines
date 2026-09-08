using System.Data.Common;
using CoreDomain;
using Npgsql;


namespace ArticleDatabase;

public class Coordinator
{
    private IDictionary<string, DbConnection> ConnectionCache = new Dictionary<string, DbConnection>();
    
    // Database for each continent + global database
    private DatabaseStruct ASIA_DB = new DatabaseStruct { name = "asia", port = "5434" };
    private DatabaseStruct EUROPE_DB = new DatabaseStruct { name = "europe", port = "5433" };
    private DatabaseStruct NORTH_AMERICA_DB = new DatabaseStruct { name = "north_america", port = "5435" };
    private DatabaseStruct SOUTH_AMERICA_DB = new DatabaseStruct { name = "south_america", port = "5436" };
    private DatabaseStruct AFRICA_DB = new DatabaseStruct { name = "africa", port = "5432" };
    private DatabaseStruct OCEANIA_DB = new DatabaseStruct { name = "oceania", port = "5438" };
    private DatabaseStruct ANTARCTICA_DB = new DatabaseStruct { name = "antarctica", port = "5437" };
    private DatabaseStruct GLOBAL_DB = new DatabaseStruct { name = "global", port = "5439" };

    
    
    public List<DbConnection> GetAllConnections()
    {
        List<DbConnection> connections = new List<DbConnection>();

        connections.Add(new NpgsqlConnection($"Server=localhost;Port={ASIA_DB.port};Database={ASIA_DB.name};User Id=admin;Password=admin;"));
        connections.Add(new NpgsqlConnection($"Server=localhost;Port={EUROPE_DB.port};Database={EUROPE_DB.name};User Id=admin;Password=admin;"));
        connections.Add(new NpgsqlConnection($"Server=localhost;Port={NORTH_AMERICA_DB.port};Database={NORTH_AMERICA_DB.name};User Id=admin;Password=admin;"));
        connections.Add(new NpgsqlConnection($"Server=localhost;Port={SOUTH_AMERICA_DB.port};Database={SOUTH_AMERICA_DB.name};User Id=admin;Password=admin;"));
        connections.Add(new NpgsqlConnection($"Server=localhost;Port={AFRICA_DB.port};Database={AFRICA_DB.name};User Id=admin;Password=admin;"));
        connections.Add(new NpgsqlConnection($"Server=localhost;Port={OCEANIA_DB.port};Database={OCEANIA_DB.name};User Id=admin;Password=admin;"));
        connections.Add(new NpgsqlConnection($"Server=localhost;Port={ANTARCTICA_DB.port};Database={ANTARCTICA_DB.name};User Id=admin;Password=admin;"));
        connections.Add(new NpgsqlConnection($"Server=localhost;Port={GLOBAL_DB.port};Database={GLOBAL_DB.name};User Id=admin;Password=admin;"));

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
        
        connection = new NpgsqlConnection($"Server=localhost;Port={databaseStruct.port};Database={databaseStruct.name};User Id=admin;Password=admin;");
        connection.Open();
        ConnectionCache.Add(databaseStruct.name, connection);
        return connection;
    }
}