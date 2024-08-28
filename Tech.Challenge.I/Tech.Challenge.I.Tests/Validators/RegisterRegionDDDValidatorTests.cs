using FluentValidation.TestHelper;
using Tech.Challenge.I.Application.UseCase.DDD.Register;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Communication.Request.Enum;
using Tech.Challenge.I.Exceptions;

namespace Tech.Challenge.I.Tests.Validators;
public class RegisterRegionDDDValidatorTests
{
    private readonly RegisterRegionDDDValidator _validator;

    public RegisterRegionDDDValidatorTests()
    {
        _validator = new RegisterRegionDDDValidator();
    }

    [Fact]
    public void Should_NotHaveError_When_RequestIsValid()
    {
        // Arrange
        var model = new RequestRegionDDDJson { Region = RegionRequestEnum.Sudeste, DDD = 21 };

        var validator = new RegisterRegionDDDValidator();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_HaveError_When_DDDIsOutOfRange()
    {
        // Arrange
        var model = new RequestRegionDDDJson { Region = RegionRequestEnum.Sudeste, DDD = 100 };

        var validator = new RegisterRegionDDDValidator();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.Contains(ErrorsMessages.DDDBetweenTenNinetyNine, result.Errors.Select(e => e.ErrorMessage));
    }

    [Fact]
    public void Should_HaveError_When_DDDIsEmpty()
    {
        // Arrange
        var model = new RequestRegionDDDJson { Region = RegionRequestEnum.Norte, DDD = 0 };

        var validator = new RegisterRegionDDDValidator();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.Contains(ErrorsMessages.DDDNotFound, result.Errors.Select(e => e.ErrorMessage));
    }

    [Fact]
    public void Should_HaveError_When_RegionIsInvalid()
    {
        // Arrange
        var model = new RequestRegionDDDJson { Region = (RegionRequestEnum)999, DDD = 21 };

        var validator = new RegisterRegionDDDValidator();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        Assert.Contains(ErrorsMessages.InvalidRegion, result.Errors.Select(e => e.ErrorMessage));
    }
}
