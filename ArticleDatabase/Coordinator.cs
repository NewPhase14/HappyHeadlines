using Microsoft.EntityFrameworkCore;

namespace ArticleDatabase;

public class Coordinator
{
    private readonly Dictionary<string, string?> _connectionStrings = new()
    {
        ["Asia"] = Environment.GetEnvironmentVariable("ASIA_DB_CONNECTION"),
        ["Europe"] = Environment.GetEnvironmentVariable("EUROPE_DB_CONNECTION"),
        ["North America"] = Environment.GetEnvironmentVariable("NORTH_AMERICA_DB_CONNECTION"),
        ["South America"] = Environment.GetEnvironmentVariable("SOUTH_AMERICA_DB_CONNECTION"),
        ["Africa"] = Environment.GetEnvironmentVariable("AFRICA_DB_CONNECTION"),
        ["Oceania"] = Environment.GetEnvironmentVariable("OCEANIA_DB_CONNECTION"),
        ["Antarctica"] = Environment.GetEnvironmentVariable("ANTARCTICA_DB_CONNECTION"),
        ["Global"] = Environment.GetEnvironmentVariable("GLOBAL_DB_CONNECTION")
    };

    public ArticleContext GetContext(string continent)
    {
        if (!_connectionStrings.TryGetValue(continent, out var connectionString))
            throw new ArgumentException($"Invalid continent: {continent}");

        var options = new DbContextOptionsBuilder<ArticleContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ArticleContext(options);
    }

    public IEnumerable<ArticleContext> GetAllContexts()
    {
        return _connectionStrings.Values.Select(connectionString => new DbContextOptionsBuilder<ArticleContext>()
            .UseNpgsql(connectionString)
            .Options).Select(options => new ArticleContext(options));
    }
}