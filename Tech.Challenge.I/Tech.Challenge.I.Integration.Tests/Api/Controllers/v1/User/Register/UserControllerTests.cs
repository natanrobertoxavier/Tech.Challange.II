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

        var request = new RequestLoginJsonBuilder()
            .WithEmail(user.Email)
            .WithPassword(password)
            .Build();

        // Act
        var response = await Client.PostAsJsonAsync(ControllerUri, request);


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadAsStringAsync();
    }
}
