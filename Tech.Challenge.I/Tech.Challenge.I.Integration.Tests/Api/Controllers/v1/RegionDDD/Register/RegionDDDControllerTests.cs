using FluentAssertions;
using System.Net;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Integration.Tests.Fakes.Request;

namespace Tech.Challenge.I.Integration.Tests.Api.Controllers.v1.RegionDDD.Register;
public class RegionDDDControllerTests() : BaseTestClient("")
{
    private const string REGISTER_DDD = "/api/v1/regionddd";

    [Fact]
    public async Task RegionDDDController_Created_WhenDDDRegisteredSuccessfully()
    {
        // Arrange
        var user = Factory.RecoverUser();
        var password = Factory.RecoverPassword();

        var token = await Login(user.Email, password);

        var request = new RequestRegionDDDJsonBuilder()
            .Build();

        // Act
        var response = await PostRequest(REGISTER_DDD, request, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task RegionDDDController_Unauthorized_WhenInvalidToken()
    {
        // Arrange
        var token = string.Empty;

        var request = new RequestRegionDDDJsonBuilder()
            .Build();

        // Act
        var response = await PostRequest(REGISTER_DDD, request, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        result.Contains(ErrorsMessages.UserWithoutPermission);
    }

    [Fact]
    public async Task RegionDDDController_Bad_WhenInvalidDDD()
    {
        // Arrange
        var user = Factory.RecoverUser();
        var password = Factory.RecoverPassword();

        var token = await Login(user.Email, password);

        var request = new RequestRegionDDDJsonBuilder()
            .WithDDD(100)
            .Build();

        // Act
        var response = await PostRequest(REGISTER_DDD, request, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Contains(ErrorsMessages.DDDBetweenTenNinetyNine);
    }
}
