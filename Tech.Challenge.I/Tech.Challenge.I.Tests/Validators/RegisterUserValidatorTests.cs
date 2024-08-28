using FluentValidation.TestHelper;
using Tech.Challenge.I.Application.UseCase.User.Register;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Exceptions;

namespace Tech.Challenge.I.Tests.Validators;
public class RegisterUserValidatorTests
{
    private readonly RegisterUserValidator _validator;

    public RegisterUserValidatorTests()
    {
        _validator = new RegisterUserValidator();
    }

    [Fact]
    public void Should_NotHaveError_When_RequestIsValid()
    {
        // Arrange
        var model = new RequestRegisterUserJson { Name = "John Doe", Email = "user@example.com", Password = "ValidPass123" };

        var validator = new RegisterUserValidator();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_HaveError_When_PasswordIsInvalid()
    {
        // Arrange
        var model = new RequestRegisterUserJson { Name = "John Doe", Email = "user@example.com", Password = "short" };

        var validator = new RegisterUserValidator();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.Contains(ErrorsMessages.MinimumSixCharacters, result.Errors.Select(e => e.ErrorMessage));
    }

    [Fact]
    public void Should_HaveError_When_EmailIsInvalid()
    {
        // Arrange
        var model = new RequestRegisterUserJson { Name = "John Doe", Email = "invalid-email", Password = "ValidPass123" };

        var validator = new RegisterUserValidator();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.Contains(ErrorsMessages.InvalidUserEmail, result.Errors.Select(e => e.ErrorMessage));
    }

    [Fact]
    public void Should_HaveError_When_EmailIsEmpty()
    {
        // Arrange
        var model = new RequestRegisterUserJson { Name = "John Doe", Email = string.Empty, Password = "ValidPass123" };

        var validator = new RegisterUserValidator();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.Contains(ErrorsMessages.BlankUserEmail, result.Errors.Select(e => e.ErrorMessage));
    }

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        // Arrange
        var model = new RequestRegisterUserJson { Name = string.Empty, Email = "user@example.com", Password = "ValidPass123" };

        var validator = new RegisterUserValidator();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.Contains(ErrorsMessages.BlankUserName, result.Errors.Select(e => e.ErrorMessage));
    }
}
