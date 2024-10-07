using FluentAssertions;
using System.Net;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Integration.Tests.Fakes.Request;

namespace Tech.Challenge.I.Integration.Tests.Api.Controllers.v1.Contact.Delete;
public class ContactControllerTests() : BaseTestClient("")
{
    private const string URI_REGION_DDD = "/api/v1/regionddd";
    private const string URI_CONTACT = "/api/v1/contact";
    private const string URI_DELETE_CONTACT = "/api/v1/contact?id={0}";

    [Fact]
    public async Task ContactController_NoContent_WhenContactIsUpdated()
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

        var contactRegistered = await GetContactRegistered(await GetRequest(URI_CONTACT, token));

        var uri = string.Format(URI_DELETE_CONTACT, contactRegistered.ContactId);

        // Act
        var response = await DeleteRequest(uri, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ContactController_UnprocessableEntity_WhenContactNotFound()
    {
        // Arrange
        var user = Factory.RecoverUser();
        var password = Factory.RecoverPassword();

        var token = await Login(user.Email, password);

        var uri = string.Format(URI_DELETE_CONTACT, Guid.NewGuid());

        // Act
        var response = await DeleteRequest(uri, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableContent);
        result.Should().NotBeNullOrEmpty();
        result.Contains(ErrorsMessages.NoContactsFound);
    }

    [Fact]
    public async Task ContactController_Unauthorized_WhenInvalidToken()
    {
        // Arrange
        var token = string.Empty;

        var uri = string.Format(URI_DELETE_CONTACT, Guid.NewGuid());

        // Act
        var response = await DeleteRequest(uri, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        result.Should().NotBeNullOrEmpty();
        result.Contains(ErrorsMessages.UserWithoutPermission);
    }

    private static async Task<ResponseContactJson> GetContactRegistered(HttpResponseMessage httpResponseMessage)
    {
        var response = await DeserializeResponse<IEnumerable<ResponseContactJson>>(httpResponseMessage);

        return response.FirstOrDefault() ?? new ResponseContactJson();
    }
}
