using Azure;
using FluentAssertions;
using System.Net;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Integration.Tests.Fakes.Request;

namespace Tech.Challenge.I.Integration.Tests.Api.Controllers.v1.Contact.Update;
public class ContactControllerTests() : BaseTestClient("")
{
    private const string URI_REGION_DDD = "/api/v1/regionddd";
    private const string URI_CONTACT = "/api/v1/contact";
    private const string URI_UPDATE_CONTACT = "/api/v1/contact?id={0}";

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

        var requestUpdateContact = new RequestContactJsonBuilder()
            .WithFirstName("New Name")
            .WithLastName(contactRegistered.LastName)
            .WithDDD(contactRegistered.DDD)
            .WithPhoneNumber(contactRegistered.PhoneNumber)
            .WithEmail(contactRegistered.Email)
            .Build();

        var uri = string.Format(URI_UPDATE_CONTACT, contactRegistered.ContactId);

        // Act
        var response = await PutRequest(uri, requestUpdateContact, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ContactController_Unauthorized_WhenInvalidToken()
    {
        // Arrange
        var token = string.Empty;

        var requestUpdateContact = new RequestContactJsonBuilder()
            .Build();

        var uri = string.Format(URI_UPDATE_CONTACT, Guid.NewGuid());

        // Act
        var response = await PutRequest(uri, requestUpdateContact, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ContactController_BadRequest_WhenFirstNameInBlank()
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

        var requestUpdateContact = new RequestContactJsonBuilder()
            .WithFirstName(string.Empty)
            .WithLastName(contactRegistered.LastName)
            .WithDDD(contactRegistered.DDD)
            .WithPhoneNumber(contactRegistered.PhoneNumber)
            .WithEmail(contactRegistered.Email)
            .Build();

        var uri = string.Format(URI_UPDATE_CONTACT, contactRegistered.ContactId);

        // Act
        var response = await PutRequest(uri, requestUpdateContact, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Contains(ErrorsMessages.BlankFirstName);
    }

    [Fact]
    public async Task ContactController_BadRequest_WhenLastNameInBlank()
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

        var requestUpdateContact = new RequestContactJsonBuilder()
            .WithFirstName(contactRegistered.FirstName)
            .WithLastName(string.Empty)
            .WithDDD(contactRegistered.DDD)
            .WithPhoneNumber(contactRegistered.PhoneNumber)
            .WithEmail(contactRegistered.Email)
            .Build();

        var uri = string.Format(URI_UPDATE_CONTACT, contactRegistered.ContactId);

        // Act
        var response = await PutRequest(uri, requestUpdateContact, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Contains(ErrorsMessages.BlankLastName);
    }

    [Fact]
    public async Task ContactController_BadRequest_WhenDDDNotFound()
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

        var requestUpdateContact = new RequestContactJsonBuilder()
            .WithFirstName(contactRegistered.FirstName)
            .WithLastName(contactRegistered.LastName)
            .WithDDD(88)
            .WithPhoneNumber(contactRegistered.PhoneNumber)
            .WithEmail(contactRegistered.Email)
            .Build();

        var uri = string.Format(URI_UPDATE_CONTACT, contactRegistered.ContactId);

        // Act
        var response = await PutRequest(uri, requestUpdateContact, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Contains(ErrorsMessages.DDDNotFound);
    }

    [Fact]
    public async Task ContactController_BadRequest_WhenPhoneNumberIsEmpty()
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

        var requestUpdateContact = new RequestContactJsonBuilder()
            .WithFirstName(contactRegistered.FirstName)
            .WithLastName(contactRegistered.LastName)
            .WithDDD(contactRegistered.DDD)
            .WithPhoneNumber(string.Empty)
            .WithEmail(contactRegistered.Email)
            .Build();

        var uri = string.Format(URI_UPDATE_CONTACT, contactRegistered.ContactId);

        // Act
        var response = await PutRequest(uri, requestUpdateContact, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Contains(ErrorsMessages.BlankPhoneNumber);
    }

    [Fact]
    public async Task ContactController_BadRequest_WhenEmailIsEmpty()
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

        var requestUpdateContact = new RequestContactJsonBuilder()
            .WithFirstName(contactRegistered.FirstName)
            .WithLastName(contactRegistered.LastName)
            .WithDDD(contactRegistered.DDD)
            .WithPhoneNumber(contactRegistered.PhoneNumber)
            .WithEmail(string.Empty)
            .Build();

        var uri = string.Format(URI_UPDATE_CONTACT, contactRegistered.ContactId);

        // Act
        var response = await PutRequest(uri, requestUpdateContact, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Contains(ErrorsMessages.BlankEmail);
    }

    [Fact]
    public async Task ContactController_BadRequest_WhenInvalidEmail()
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

        var requestUpdateContact = new RequestContactJsonBuilder()
            .WithFirstName(contactRegistered.FirstName)
            .WithLastName(contactRegistered.LastName)
            .WithDDD(contactRegistered.DDD)
            .WithPhoneNumber(contactRegistered.PhoneNumber)
            .WithEmail("invalid-email")
            .Build();

        var uri = string.Format(URI_UPDATE_CONTACT, contactRegistered.ContactId);

        // Act
        var response = await PutRequest(uri, requestUpdateContact, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Contains(ErrorsMessages.InvalidEmail);
    }

    [Fact]
    public async Task ContactController_BadRequest_WhenInvalidPhoneNumber()
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

        var requestUpdateContact = new RequestContactJsonBuilder()
            .WithFirstName(contactRegistered.FirstName)
            .WithLastName(contactRegistered.LastName)
            .WithDDD(contactRegistered.DDD)
            .WithPhoneNumber("99-120")
            .WithEmail(contactRegistered.Email)
            .Build();

        var uri = string.Format(URI_UPDATE_CONTACT, contactRegistered.ContactId);

        // Act
        var response = await PutRequest(uri, requestUpdateContact, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Contains(ErrorsMessages.InvalidPhoneNumber);
    }

    private static async Task<ResponseContactJson> GetContactRegistered(HttpResponseMessage httpResponseMessage)
    {
        var response = await DeserializeResponse<IEnumerable<ResponseContactJson>>(httpResponseMessage);

        return response.FirstOrDefault() ?? new ResponseContactJson();
    }
}
