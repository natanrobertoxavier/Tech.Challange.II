using System.Net.Http.Json;
using FluentAssertions;
using System.Net;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Integration.Tests.Fakes.Request;

namespace Tech.Challenge.I.Integration.Tests.Api.Controllers.v1.Login;
public class LoginControllerTests() : BaseTestClient("/api/v1/login")
{
    [Fact]
    public async Task Login_ReturnsOk_WhenSuccessfulLogin()
    {
        // Arrange
        var user = Factory.RecoverUser();
        var password = Factory.RecoverPassword();

        var request = new RequestLoginJsonBuilder()
            .WithEmail(user.Email)
            .WithPassword(password)
            .Build();

        // Act
        var response = await Client.PostAsJsonAsync(ControllerUri, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var loggedUserJson = await DeserializeResponse<ResponseLoginJson>(response);

        loggedUserJson.Name.Should().NotBeNullOrWhiteSpace();
        loggedUserJson.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenLoginFail_IncorrectPassword()
    {
        // Arrange
        var user = Factory.RecoverUser();

        var request = new RequestLoginJsonBuilder()
            .WithEmail(user.Email)
            .WithPassword("incorrec-password")
            .Build();

        // Act
        var response = await Client.PostAsJsonAsync(ControllerUri, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var loggedUserJson = await DeserializeResponse<ResponseLoginJson>(response);

        loggedUserJson.Name.Should().BeNull();
        loggedUserJson.Token.Should().BeNull();
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenLoginFail_IncorrectEmail()
    {
        // Arrange
        var password = Factory.RecoverPassword();

        var request = new RequestLoginJsonBuilder()
            .WithEmail("incorrec-email@email.com")
            .WithPassword(password)
            .Build();

        // Act
        var response = await Client.PostAsJsonAsync(ControllerUri, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var loggedUserJson = await DeserializeResponse<ResponseLoginJson>(response);

        loggedUserJson.Name.Should().BeNull();
        loggedUserJson.Token.Should().BeNull();
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenLoginFail_IncorrectPasswordAndEmail()
    {
        // Arrange
        var request = new RequestLoginJsonBuilder()
            .WithEmail("incorrec-email@email.com")
            .WithPassword("incorrec-password")
            .Build();

        // Act
        var response = await Client.PostAsJsonAsync(ControllerUri, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var loggedUserJson = await DeserializeResponse<ResponseLoginJson>(response);

        loggedUserJson.Name.Should().BeNull();
        loggedUserJson.Token.Should().BeNull();
    }
}
