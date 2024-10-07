using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tech.Challenge.I.Api.Controllers.v1;
using Tech.Challenge.I.Application.UseCase.User.ChangePassword;
using Tech.Challenge.I.Application.UseCase.User.Register;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Tests.Controllers;

public class UserControllerTests
{
    [Fact]
    public async Task RegisterUser_ReturnsCreatedResult_WithValidRequest()
    {
        // Arrange
        var mockUseCase = new Mock<IRegisterUserUseCase>();

        var request = new RequestRegisterUserJson
        {
            Email = "new@email.com",
            Password = "password",
            Name = "name",
        };

        var response = new ResponseRegisteredUserJson
        {
            Token = "valid_token"
        };

        mockUseCase.Setup(useCase => useCase.Execute(It.IsAny<RequestRegisterUserJson>()))
                   .ReturnsAsync(response);

        var controller = new UserController();

        // Act
        var result = await controller.RegisterUser(mockUseCase.Object, request) as CreatedResult;

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(nameof(LoginController.Login), result.Location);
        Assert.Equal(response, result.Value);
    }

    [Fact]
    public async Task RegisterUser_ThrowsValidationErrorsException_WhenRequestIsInvalid()
    {
        // Arrange
        var mockUseCase = new Mock<IRegisterUserUseCase>();
        var request = new RequestRegisterUserJson
        {
            Name = "",
            Email = "",
            Password = ""
        };

        var validationErrors = new ValidationErrorsException(new List<string>
        {
            "Nome do usuário em branco",
            "Email do usuário em branco",
            "Senha do usuário em branco"
        });

        mockUseCase.Setup(useCase => useCase.Execute(request))
                   .ThrowsAsync(validationErrors);

        var controller = new UserController();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationErrorsException>(() =>
            controller.RegisterUser(mockUseCase.Object, request));

        Assert.Equal(validationErrors.ErrorMessages, exception.ErrorMessages);
    }

    [Fact]
    public async Task ChangePassword_ReturnsNoContentResult_WhenRequestIsValid()
    {
        // Arrange
        var mockUseCase = new Mock<IChangePasswordUseCase>();
        var request = new RequestChangePasswordJson
        {
            CurrentPassword = "current_password",
            NewPassword = "new_password"
        };

        mockUseCase.Setup(useCase => useCase.Execute(request))
                   .Returns(Task.CompletedTask);

        var controller = new UserController();

        // Act
        var result = await controller.ChangePassword(mockUseCase.Object, request) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_ThrowsInvalidCurrentPasswordException_WhenCurrentPasswordIsInvalid()
    {
        // Arrange
        var mockUseCase = new Mock<IChangePasswordUseCase>();
        var request = new RequestChangePasswordJson
        {
            CurrentPassword = "wrong_current_password",
            NewPassword = "new_password"
        };

        var invalidPasswordException = new ValidationErrorsException(new List<string>
        {
            "Senha atual incorreta",
        });

        mockUseCase.Setup(useCase => useCase.Execute(request))
                   .ThrowsAsync(invalidPasswordException);

        var controller = new UserController();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationErrorsException>(() =>
            controller.ChangePassword(mockUseCase.Object, request));

        Assert.Contains("Senha atual incorreta", exception.ErrorMessages);
    }

    [Fact]
    public async Task ChangePassword_ReturnsUnauthorizedResult_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var mockUseCase = new Mock<IChangePasswordUseCase>();

        var request = new RequestChangePasswordJson
        {
            CurrentPassword = "current_password",
            NewPassword = "new_password"
        };

        var unauthorizedException = new ValidationErrorsException(new List<string>
        {
            "Usuário sem permissão",
        });

        mockUseCase.Setup(useCase => useCase.Execute(request))
                   .ThrowsAsync(unauthorizedException);

        var controller = new UserController();

        // Act
        var result = await Assert.ThrowsAsync<ValidationErrorsException>(() =>
            controller.ChangePassword(mockUseCase.Object, request));


        // Assert
        Assert.NotNull(result);
        Assert.Contains("Usuário sem permissão", result.ErrorMessages);
    }
}
