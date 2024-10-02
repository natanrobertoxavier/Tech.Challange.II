using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Tech.Challenge.I.Integration.Tests.WebAPI.Controllers.Base;
public class IntegrationTestFixture : IAsyncLifetime
{
    public async Task DisposeAsync()
    {
        await DropDatabaseAsync();
    }

    private static async Task DropDatabaseAsync()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
        var databaseName = Environment.GetEnvironmentVariable("IntegrationTestDatabase");

        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = $"DROP DATABASE IF EXISTS `{databaseName}`;";
        await command.ExecuteNonQueryAsync();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}
