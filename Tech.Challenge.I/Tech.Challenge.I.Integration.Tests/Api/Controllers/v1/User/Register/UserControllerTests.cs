using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.Arm;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Integration.Tests.Fakes.Request;

namespace Tech.Challenge.I.Integration.Tests.Api.Controllers.v1.User.Register;
public class UserControllerTests() : BaseTestClient("/api/v1/user")
{
    [Fact]
    public async Task UserController_ReturnsCreated_WhenUserIsCreated()
    {
        // Arrange
        var request = new RequestRegisterUserJsonBuilder()
            .Build();

        // Act
        var response = await Client.PostAsJsonAsync(ControllerUri, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var userRegistredJson = await DeserializeResponse<ResponseRegisteredUserJson>(response);

        userRegistredJson.Should().NotBeNull();
        userRegistredJson.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UserController_ReturnsError_WhenUserAlreadyExists()
    {
        // Arrange
        var user = Factory.RecoverUser();
        var password = Factory.RecoverPassword();

        var request = new RequestRegisterUserJsonBuilder()
            .WithEmail(user.Email)
            .WithPassword(password)
            .Build();

        // Act
        var response = await Client.PostAsJsonAsync(ControllerUri, request);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        result.Should().NotBeNull();
        result.Contains(ErrorsMessages.EmailAlreadyRegistered);
    }

    [Fact]
    public async Task UserController_ReturnsError_WhenUserEmailIsBlank()
    {
        // Arrange
        var request = new RequestRegisterUserJsonBuilder()
            .WithName(string.Empty)
            .Build();

        // Act
        var response = await Client.PostAsJsonAsync(ControllerUri, request);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        result.Should().NotBeNull();
        result.Contains(ErrorsMessages.BlankUserEmail);
    }

    [Fact]
    public async Task UserController_ReturnsError_WhenPasswordIsBlank()
    {
        // Arrange
        var request = new RequestRegisterUserJsonBuilder()
            .WithPassword(string.Empty)
            .Build();

        // Act
        var response = await Client.PostAsJsonAsync(ControllerUri, request);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        result.Should().NotBeNull();
        result.Contains(ErrorsMessages.BlankUserPassword);
    }

    [Fact]
    public async Task UserController_ReturnsError_WhenPasswordIsInvalid()
    {
        // Arrange
        var request = new RequestRegisterUserJsonBuilder()
            .WithPassword("12345")
            .Build();

        // Act
        var response = await Client.PostAsJsonAsync(ControllerUri, request);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        result.Should().NotBeNull();
        result.Contains(ErrorsMessages.MinimumSixCharacters);
    }

    [Fact]
    public async Task UserController_ReturnsError_WhenEmailIsInvalid()
    {
        // Arrange
        var request = new RequestRegisterUserJsonBuilder()
            .WithEmail("invaid-email")
            .Build();

        // Act
        var response = await Client.PostAsJsonAsync(ControllerUri, request);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        result.Should().NotBeNull();
        result.Contains(ErrorsMessages.InvalidUserEmail);
    }
}
