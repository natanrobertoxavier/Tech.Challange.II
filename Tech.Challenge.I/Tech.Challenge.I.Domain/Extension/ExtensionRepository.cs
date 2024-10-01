using Microsoft.Extensions.Configuration;

namespace Tech.Challenge.I.Domain.Extension;

public static class ExtensionRepository
{
    public static string GetConnection(this IConfiguration configurationManager)
    {
        var connection = configurationManager.GetConnectionString("Connection");
        return connection;
    }

    public static string GetDatabaseName(this IConfiguration configurationManager)
    {
        var databaseName = configurationManager.GetConnectionString("DatabaseName");
        return databaseName;
    }

    public static string GetIntegrationTestDatabaseName(this IConfiguration configurationManager)
    {
        var databaseName = configurationManager.GetConnectionString("IntegrationTestDatabase");
        return databaseName;
    }

    public static string GetFullConnection(this IConfiguration configurationManager)
    {
        var databaseName = configurationManager.GetDatabaseName();
        var connection = configurationManager.GetConnection();

        return $"{connection}Database={databaseName};";
    }

    public static string GetFullConnectionIntegrationTest(this IConfiguration configurationManager)
    {
        var databaseName = configurationManager.GetIntegrationTestDatabaseName();
        var connection = configurationManager.GetConnection();

        return $"{connection}Database={databaseName};";
    }
}
