using FluentAssertions;
using System.Net;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Integration.Tests.Fakes.Request;

namespace Tech.Challenge.I.Integration.Tests.Api.Controllers.v1.Contact.Register;
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

        // Act
        var response = await PostRequest(URI_CONTACT, requestRegisterContact, token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task ContactController_Unauthorized_WhenInvalidToken()
    {
        // Arrange
        var token = string.Empty;

        var requestRegisterContact = new RequestContactJsonBuilder()
            .Build();

        // Act
        var response = await PostRequest(URI_CONTACT, requestRegisterContact, token);

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
            .WithFirstName(string.Empty)
            .Build();

        // Act
        var response = await PostRequest(URI_CONTACT, requestRegisterContact, token);

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
            .WithLastName(string.Empty)
            .Build();

        // Act
        var response = await PostRequest(URI_CONTACT, requestRegisterContact, token);

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
            .WithDDD(14)
            .Build();

        // Act
        var response = await PostRequest(URI_CONTACT, requestRegisterContact, token);

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
            .WithPhoneNumber(string.Empty)
            .Build();

        // Act
        var response = await PostRequest(URI_CONTACT, requestRegisterContact, token);

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
            .WithEmal(string.Empty)
            .Build();

        // Act
        var response = await PostRequest(URI_CONTACT, requestRegisterContact, token);

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
            .WithEmal("invalid-email")
            .Build();

        // Act
        var response = await PostRequest(URI_CONTACT, requestRegisterContact, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Contains(ErrorsMessages.BlankEmail);
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
            .WithPhoneNumber("99-0000")
            .Build();

        // Act
        var response = await PostRequest(URI_CONTACT, requestRegisterContact, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Contains(ErrorsMessages.BlankEmail);
    }

    [Fact]
    public async Task ContactController_BadRequest_WhenContactAlreadyRegistered()
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
        var response = await PostRequest(URI_CONTACT, requestRegisterContact, token);

        // Assert
        var result = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Contains(ErrorsMessages.ContactAlreadyRegistered);
    }
}
