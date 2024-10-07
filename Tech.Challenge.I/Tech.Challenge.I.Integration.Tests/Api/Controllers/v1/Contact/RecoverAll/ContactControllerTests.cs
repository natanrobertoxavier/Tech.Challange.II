using FluentAssertions;
using System.Net;
using Tech.Challenge.I.Integration.Tests.Fakes.Request;

namespace Tech.Challenge.I.Integration.Tests.Api.Controllers.v1.Contact.RecoverAll;
public class ContactControllerTests() : BaseTestClient("")
{
    private const string URI_REGION_DDD = "/api/v1/regionddd";
    private const string URI_CONTACT = "/api/v1/contact";

    [Fact]
    public async Task ContactController_OK_WhenContactIsCreated()
    {
        // Arrange
        var user = Factory.RecoverUser();
        var password = Factory.RecoverPassword();

        var token = await Login(user.Email, password);

        var requestRegisterDDD = new RequestRegionDDDJsonBuilder()
            .Build();

        await PostRequest(URI_REGION_DDD, requestRegisterDDD, token);

        var requestRegisterContact = new RequestContactJsonBuilder()
            .Build();

        await PostRequest(URI_CONTACT, requestRegisterContact, token);

        // Act
        var response = await GetRequest(URI_CONTACT, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNullOrEmpty();
        result.Contains("ContactId");
        result.Contains("Region");
        result.Contains("FirstName");
        result.Contains("LastName");
        result.Contains("DDD");
        result.Contains("PhoneNumber");
        result.Contains("Email");
    }

    [Fact]
    public async Task ContactController_NoContent_WhenContactNotFound()
    {
        // Arrange
        var user = Factory.RecoverUser();
        var password = Factory.RecoverPassword();

        var token = await Login(user.Email, password);

        // Act
        var response = await GetRequest(URI_CONTACT, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        result.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task ContactController_Unauthorized_WhenInvalidToken()
    {
        // Arrange
        var token = string.Empty;

        // Act
        var response = await GetRequest(URI_CONTACT, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
