using FluentAssertions;
using System.Net;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Integration.Tests.Fakes.Request;

namespace Tech.Challenge.I.Integration.Tests.Api.Controllers.v1.User.ChangePassword;
public class UserControllerTests() : BaseTestClient("")
{
    private const string CHANGE_PASSWORD = "/api/v1/user/change-password";

    [Fact]
    public async Task UserController_NoContent_WhenPasswordIsChangedSuccessfully()
    {
        // Arrange
        var user = Factory.RecoverUser();
        var password = Factory.RecoverPassword();

        var token = await Login(user.Email, password);

        var request = new RequestChangePasswordJsonBuilder()
            .WithCurrentPassword(password)
            .Build();

        // Act
        var resposta = await PutRequest(CHANGE_PASSWORD, request, token);

        // Assert
        resposta.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UserController_BadRequest_WhenCurrentPasswordInBlank()
    {
        // Arrange
        var user = Factory.RecoverUser();
        var password = Factory.RecoverPassword();

        var token = await Login(user.Email, password);

        var request = new RequestChangePasswordJsonBuilder()
            .WithCurrentPassword(string.Empty)
            .Build();

        // Act
        var response = await PutRequest(CHANGE_PASSWORD, request, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        result.Should().NotBeNull();
        result.Contains(ErrorsMessages.InvalidCurrentPassword);
    }

    [Fact]
    public async Task UserController_BadRequest_WhenInvalidNewPasswor()
    {
        // Arrange
        var user = Factory.RecoverUser();
        var password = Factory.RecoverPassword();

        var token = await Login(user.Email, password);

        var request = new RequestChangePasswordJsonBuilder()
            .WithCurrentPassword(password)
            .WithNewPassword(string.Empty)
            .Build();

        // Act
        var response = await PutRequest(CHANGE_PASSWORD, request, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        result.Should().NotBeNull();
        result.Contains(ErrorsMessages.MinimumSixCharacters);
    }
}