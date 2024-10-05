using FluentAssertions;
using System.Net;
using Tech.Challenge.I.Integration.Tests.Fakes.Request;

namespace Tech.Challenge.I.Integration.Tests.Api.Controllers.v1.RegionDDD.RecoverAll;
public class RegionDDDControllerTests() : BaseTestClient("")
{
    private const string URI_REGION_DDD = "/api/v1/regionddd";

    [Fact]
    public async Task RegionDDDController_OK_WhenDDDFound()
    {
        // Arrange
        var user = Factory.RecoverUser();
        var password = Factory.RecoverPassword();

        var token = await Login(user.Email, password);

        var requestRegisterDDD = new RequestRegionDDDJsonBuilder()
            .Build();

        await PostRequest(URI_REGION_DDD, requestRegisterDDD, token);

        // Act
        var response = await GetRequest(URI_REGION_DDD, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNullOrEmpty();
        result.Contains("DDD");
        result.Contains("Region");
    }

    [Fact]
    public async Task RegionDDDController_NoContent_WhenDDDNotFound()
    {
        // Arrange
        var user = Factory.RecoverUser();
        var password = Factory.RecoverPassword();

        var token = await Login(user.Email, password);

        // Act
        var response = await GetRequest(URI_REGION_DDD, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        result.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task RegionDDDController_Unauthorized_WhenInvalidToken()
    {
        // Arrange
        var token = string.Empty;

        // Act
        var response = await GetRequest(URI_REGION_DDD, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
