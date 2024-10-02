using Dapper;
using MySqlConnector;
using System.Diagnostics.CodeAnalysis;

namespace Tech.Challenge.I.Infrastructure.Migrations;

[ExcludeFromCodeCoverage]
public class Database
{
    public static void CreateDatabase(string connectionWithDatabase, string databaseName)
    {
        using var myConnection = new MySqlConnection(connectionWithDatabase);

        var parameters = new DynamicParameters();
        parameters.Add("name", databaseName);

        var registry = myConnection.Query("SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @name", parameters);

        if (!registry.Any())
        {
            var result = myConnection.Execute($"CREATE DATABASE {databaseName}");

            if (result == 0) throw new Exception("An error occurred while creating the database");
        }
    }
}
