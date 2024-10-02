using Newtonsoft.Json;
using System.Text;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Integration.Tests.Fakes.Request;
using Tech.Challenge.I.Integration.Tests.WebAPI.Controllers.Base;

namespace Tech.Challenge.I.Integration.Tests.WebAPI.Controllers;
public class UserControllerTest : BaseControllerTest, IClassFixture<IntegrationTestFixture>
{
    private static string UrlBase => Path.Combine($"{ApiUrl}", "user");

    public UserControllerTest() : base()
    {
    }

    [Fact]
    public async Task UserController_ReturnsCreated_WhenUserIsCreated()
    {
        // Arrange
        var url = UrlBase;
        var request = new RequestRegisterUserJsonBuilder().Build();
        var jsonContent = SerializeObject(request);

        // Act
        var response = await _httpClient.PostAsync(url, jsonContent);

        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("token", result);
    }

    [Fact]
    public async Task UserController_ReturnsError_WhenUserAlreadyExists()
    {
        // Arrange
        var url = UrlBase;
        var request = new RequestRegisterUserJsonBuilder().Build();
        var jsonContent = SerializeObject(request);

        // Act
        var response = await _httpClient.PostAsync(url, jsonContent);

        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Contains(ErrorsMessages.EmailAlreadyRegistered, result);
    }

    [Fact]
    public async Task UserController_ReturnsError_WhenUserNameIsBlank()
    {
        // Arrange
        var url = UrlBase;
        var request = new RequestRegisterUserJsonBuilder()
            .WithName(string.Empty)
            .Build();
        var jsonContent = SerializeObject(request);

        // Act
        var response = await _httpClient.PostAsync(url, jsonContent);

        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Contains(ErrorsMessages.BlankUserName, result);
    }

    [Fact]
    public async Task UserController_ReturnsError_WhenUserEmailIsBlank()
    {
        // Arrange
        var url = UrlBase;
        var request = new RequestRegisterUserJsonBuilder()
            .WithEmail(string.Empty)
            .Build();
        var jsonContent = SerializeObject(request);

        // Act
        var response = await _httpClient.PostAsync(url, jsonContent);

        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Contains(ErrorsMessages.BlankUserEmail, result);
    }

    [Fact]
    public async Task UserController_ReturnsError_WhenPasswordIsBlank()
    {
        // Arrange
        var url = UrlBase;
        var request = new RequestRegisterUserJsonBuilder()
            .WithPassword(string.Empty)
            .Build();
        var jsonContent = SerializeObject(request);

        // Act
        var response = await _httpClient.PostAsync(url, jsonContent);

        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Contains(ErrorsMessages.BlankUserPassword, result);
    }

    [Fact]
    public async Task UserController_ReturnsError_WhenPasswordIsInvalid()
    {
        // Arrange
        var url = UrlBase;
        var request = new RequestRegisterUserJsonBuilder()
            .WithEmail("invalidpasswordtest@email.com")
            .WithPassword("12345")
            .Build();
        var jsonContent = SerializeObject(request);

        // Act
        var response = await _httpClient.PostAsync(url, jsonContent);

        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Contains(ErrorsMessages.MinimumSixCharacters, result);
    }

    [Fact]
    public async Task UserController_ReturnsError_WhenEmailIsInvalid()
    {
        // Arrange
        var url = UrlBase;
        var request = new RequestRegisterUserJsonBuilder()
            .WithEmail("invalidemail")
            .Build();
        var jsonContent = SerializeObject(request);

        // Act
        var response = await _httpClient.PostAsync(url, jsonContent);

        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Contains(ErrorsMessages.InvalidUserEmail, result);
    }

    private static StringContent SerializeObject(RequestRegisterUserJson request) => 
        new(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
}
