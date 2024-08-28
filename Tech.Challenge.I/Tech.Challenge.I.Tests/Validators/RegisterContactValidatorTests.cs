using FluentValidation.TestHelper;
using Tech.Challenge.I.Application.UseCase.Contact.Register;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Exceptions;

namespace Tech.Challenge.I.Tests.Validators;
public class RegisterContactValidatorTests
{
    private readonly RegisterContactValidator _validator;

    public RegisterContactValidatorTests()
    {
        _validator = new RegisterContactValidator();
    }

    [Fact]
    public void Should_NotHaveError_When_AllFieldsAreValid()
    {
        // Arrange
        var model = new RequestContactJson
        {
            FirstName = "John",
            LastName = "Doe",
            DDD = 11,
            PhoneNumber = "12345-6789",
            Email = "john.doe@example.com"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_When_FirstNameIsNull()
    {
        // Arrange
        var model = new RequestContactJson { FirstName = null };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        Assert.Contains(ErrorsMessages.BlankFirstName, result.Errors.Select(e => e.ErrorMessage));
    }

    [Fact]
    public void Should_HaveError_When_LastNameIsNull()
    {
        // Arrange
        var model = new RequestContactJson { LastName = null };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        Assert.Contains(ErrorsMessages.BlankLastName, result.Errors.Select(e => e.ErrorMessage));
    }

    [Fact]
    public void Should_HaveError_When_PhoneNumberIsNull()
    {
        // Arrange
        var model = new RequestContactJson { PhoneNumber = null };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        Assert.Contains(ErrorsMessages.BlankPhoneNumber, result.Errors.Select(e => e.ErrorMessage));
    }

    [Fact]
    public void Should_HaveError_When_EmailIsInvalid()
    {
        // Arrange
        var model = new RequestContactJson
        {
            FirstName = "John",
            LastName = "Doe",
            DDD = 11,
            PhoneNumber = "12345-6789",
            Email = "invalid-email"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        Assert.Contains(ErrorsMessages.InvalidEmail, result.Errors.Select(e => e.ErrorMessage));
    }

    [Fact]
    public void Should_HaveError_When_PhoneNumberIsInvalid()
    {
        // Arrange
        var model = new RequestContactJson
        {
            FirstName = "John",
            LastName = "Doe",
            DDD = 11,
            PhoneNumber = "1234-567",
            Email = "invalid-email"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        Assert.Contains(ErrorsMessages.InvalidPhoneNumber, result.Errors.Select(e => e.ErrorMessage));
    }
}
