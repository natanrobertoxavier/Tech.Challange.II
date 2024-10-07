using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tech.Challenge.I.Api.Controllers.v1;
using Tech.Challenge.I.Application.UseCase.User.Login;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Tests.Controllers;
public class LoginControllerTests
{
    [Fact]
    public async Task Login_ReturnsOkResult_WhenCredentialsAreValid()
    {
        // Arrange
        var mockUseCase = new Mock<ILoginUseCase>();

        var request = new RequestLoginJson
        {
            Email = "valid_username",
            Password = "valid_password"
        };

        var response = new ResponseLoginJson
        {
            Token = "valid_token"
        };

        mockUseCase.Setup(useCase => useCase.Execute(request))
                   .ReturnsAsync(response);

        var controller = new LoginController();

        // Act
        var result = await controller.Login(mockUseCase.Object, request) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(response, result.Value);
    }

    [Fact]
    public async Task Login_ReturnsBadRequestResult_WhenRequestIsInvalid()
    {
        // Arrange
        var mockUseCase = new Mock<ILoginUseCase>();

        var request = new RequestLoginJson
        {
            Email = "",
            Password = ""
        };

        mockUseCase.Setup(useCase => useCase.Execute(request))
                   .ThrowsAsync(new ValidationErrorsException(new List<string>
                   {
                       "E-mail ou senha incorretos"
                   }));

        var controller = new LoginController();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationErrorsException>(() =>
            controller.Login(mockUseCase.Object, request));

        // Assert
        Assert.Contains("E-mail ou senha incorretos", exception.ErrorMessages);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorizedResult_WhenCredentialsAreInvalid()
    {
        // Arrange
        var mockUseCase = new Mock<ILoginUseCase>();

        var request = new RequestLoginJson
        {
            Email = "invalid_username",
            Password = "invalid_password"
        };

        mockUseCase.Setup(useCase => useCase.Execute(request))
                   .ThrowsAsync(new InvalidLoginException());

        var controller = new LoginController();

        // Act
        var result = await Assert.ThrowsAsync<InvalidLoginException>(() =>
            controller.Login(mockUseCase.Object, request));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("E-mail ou senha incorretos", result.Message);
    }
}
