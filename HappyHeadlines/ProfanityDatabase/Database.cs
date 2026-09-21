using CoreDomain;
using Npgsql;

namespace ProfanityDatabase;

public class Database
{
    private static readonly Database instance = new Database();
    
    private readonly NpgsqlConnection connection;

    public static Database GetInstance()
    {
        return instance;
    }

    public Database()
    {
        connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("PROFANITY_DB_CONNECTION")); 
        connection.Open();
    }

    public void DeleteDatabase()
    {
        Execute("DROP TABLE IF EXISTS Profanity");
    }

    public void RecreateDatabase()
    {
        Execute("CREATE TABLE Profanity(id VARCHAR PRIMARY KEY, word VARCHAR(100))");
    }

    public void InsertWord(Profanity profanity)
    {
        Execute($"INSERT INTO Profanity(id, word) VALUES('{profanity.Id}', '{profanity.Word}')");
    }

    public bool ContainsProfanity(string message)
    {
        var words = GetWords();
        foreach (var word in words)
        {
            if (message.Contains(word, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    public List<string> GetWords()
    {
        var words = new List<string>();
        
        using var command = connection.CreateCommand();

        command.CommandText = "SELECT word FROM Profanity";

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            words.Add(reader.GetString(0));
        }

        return words;
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