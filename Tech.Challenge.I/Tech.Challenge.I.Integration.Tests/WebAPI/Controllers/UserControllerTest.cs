using Newtonsoft.Json;
using System.Text;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Integration.Tests.Fakes.Request;
using Tech.Challenge.I.Integration.Tests.WebAPI.Controllers.Base;

namespace Tech.Challenge.I.Integration.Tests.WebAPI.Controllers;
public class UserControllerTest : BaseControllerTest, IClassFixture<IntegrationTestFixture>
{
    private static string UrlBase => Path.Combine($"{ApiUrl}", "user");
    private static string UrlUser => Path.Combine($"{ApiUrl}", "login");
    private const string CHANGE_PASSWORD = "change-password";
    private const string DEFAULT_NEW_PASSWORD = "123456";
    private const string DEFAULT_CURRENT_PASSWORD = "123456";
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

    [Fact]
    public async Task UserController_NoContent_WhenPasswordIsChangedSuccessfully()
    {
        // Arrange
        var userChangePassword = await CreateUserChangePassword();

        var request = new RequestChangePasswordJsonBuilder()
            .Build();

        var jsonContent = SerializeObject(request);

        var httpRequestMessage = NewRequest(HttpMethod.Put, CHANGE_PASSWORD, jsonContent, userChangePassword.Token);

        // Act
        var response = await _httpClient.SendAsync(httpRequestMessage);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UserController_Error_WhenInvalidCurrntPassword()
    {
        // Arrange
        var userChangePassword = await CreateUserChangePassword();

        var request = new RequestChangePasswordJsonBuilder()
            .WithCurrentPassword("invalid-current-password")
            .Build();

        var jsonContent = SerializeObject(request);

        var httpRequestMessage = NewRequest(HttpMethod.Put, CHANGE_PASSWORD, jsonContent, userChangePassword.Token);

        // Act
        var response = await _httpClient.SendAsync(httpRequestMessage);

        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Contains(ErrorsMessages.InvalidCurrentPassword, result);
    }

    [Fact]
    public async Task UserController_Error_WhenNewPasswordInBlank()
    {
        // Arrange
        var userChangePassword = await CreateUserChangePassword();

        var request = new RequestChangePasswordJsonBuilder()
            .WithNewPassword(string.Empty)
            .Build();

        var jsonContent = SerializeObject(request);

        var httpRequestMessage = NewRequest(HttpMethod.Put, CHANGE_PASSWORD, jsonContent, userChangePassword.Token);

        // Act
        var response = await _httpClient.SendAsync(httpRequestMessage);

        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Contains(ErrorsMessages.BlankUserPassword, result);
    }

    [Fact]
    public async Task UserController_Error_WhenInvalidNewPassword()
    {
        // Arrange
        var userChangePassword = await CreateUserChangePassword();

        var request = new RequestChangePasswordJsonBuilder()
            .WithCurrentPassword(DEFAULT_NEW_PASSWORD)
            .WithNewPassword("12345")
            .Build();

        var jsonContent = SerializeObject(request);

        var httpRequestMessage = NewRequest(HttpMethod.Put, CHANGE_PASSWORD, jsonContent, userChangePassword.Token);

        // Act
        var response = await _httpClient.SendAsync(httpRequestMessage);

        var result = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Contains(ErrorsMessages.MinimumSixCharacters, result);
    }

    private static StringContent SerializeObject<T>(T request)
    {
        var json = JsonConvert.SerializeObject(request);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static HttpRequestMessage NewRequest(HttpMethod method, string function, StringContent jsonContent, string token)
    {
        var url = string.Concat(UrlBase, "/", function);
        var httpRequestMessage = new HttpRequestMessage(method, url)
        {
            Content = jsonContent
        };

        httpRequestMessage.Headers.Add("Authorization", $"Bearer {token}");
        return httpRequestMessage;
    }

    private async Task<ResponseRegisteredUserJson> CreateUserChangePassword()
    {
        var url = UrlBase;

        var request = new RequestRegisterUserJsonBuilder()
            .WithName("UserChangePassword")
            .WithEmail("userchangepassword@email.com")
            .WithPassword("123456")
            .Build();

        var jsonContent = SerializeObject(request);

        var response = await _httpClient.PostAsync(url, jsonContent);

        string? result;

        if (response.IsSuccessStatusCode)
        {
            result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseRegisteredUserJson>(result) ?? new();
        }

        result = await LoginUser(request.Email, DEFAULT_CURRENT_PASSWORD);

        return new ResponseRegisteredUserJson() 
        {
            Token = result,
        };
    }

    private async Task<string> LoginUser(string email, string password)
    {
        var url = UrlUser;

        var request = new RequestLoginJsonBuilder()
            .WithEmail(email)
            .WithPassword(password)
            .Build();

        var jsonContent = SerializeObject(request);

        var response = await _httpClient.PostAsync(url, jsonContent);

        var result = await response.Content.ReadAsStringAsync();

        var loggedUser = JsonConvert.DeserializeObject<ResponseLoginJson>(result) ?? new();

        return loggedUser.Token;
    }
}
