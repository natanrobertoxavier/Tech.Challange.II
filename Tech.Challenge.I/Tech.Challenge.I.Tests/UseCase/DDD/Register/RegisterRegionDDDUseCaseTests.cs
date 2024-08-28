using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Tech.Challenge.I.Application.Services.LoggedUser;
using Tech.Challenge.I.Application.UseCase.DDD.Register;
using Tech.Challenge.I.Communication;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Communication.Request.Enum;
using Tech.Challenge.I.Domain.Entities;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.DDD;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Tests.UseCase.DDD.Register;
public class RegisterRegionDDDUseCaseTests
{
    private readonly Mock<IRegionDDDReadOnlyRepository> _mockReadOnlyRepository;
    private readonly Mock<IRegionDDDWriteOnlyRepository> _mockWriteOnlyRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IWorkUnit> _mockWorkUnit;
    private readonly Mock<ILoggedUser> _mockLoggedUser;
    private readonly RegisterRegionDDDUseCase _useCase;

    public RegisterRegionDDDUseCaseTests()
    {
        _mockReadOnlyRepository = new Mock<IRegionDDDReadOnlyRepository>();
        _mockWriteOnlyRepository = new Mock<IRegionDDDWriteOnlyRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockWorkUnit = new Mock<IWorkUnit>();
        _mockLoggedUser = new Mock<ILoggedUser>();
        _useCase = new RegisterRegionDDDUseCase(
            _mockReadOnlyRepository.Object,
            _mockWriteOnlyRepository.Object,
            _mockMapper.Object,
            _mockWorkUnit.Object,
            _mockLoggedUser.Object
        );
    }

    [Fact]
    public async Task Execute_ShouldAddEntityAndCommit_WhenRequestIsValid()
    {
        // Arrange
        var request = new RequestRegionDDDJson { DDD = 11, Region = RegionRequestEnum.Sudeste };

        var entity = new RegionDDD { DDD = 11, Region = RegionRequestEnum.Sudeste.GetDescription() };

        var user = new Domain.Entities.User { Id = Guid.NewGuid(), Email = "email@email.com", Name = "McLovin" };

        _mockMapper.Setup(m => m.Map<RegionDDD>(request)).Returns(entity);
        _mockLoggedUser
            .Setup(l => l.RecoverUser())
            .ReturnsAsync(user);

        _mockReadOnlyRepository
            .Setup(r => r.ThereIsDDDNumber(request.DDD))
            .ReturnsAsync(false);

        // Act
        await _useCase.Execute(request);

        // Assert
        _mockWriteOnlyRepository.Verify(r => r.Add(entity), Times.Once);
        _mockWorkUnit.Verify(w => w.Commit(), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowValidationErrorsException_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new RequestRegionDDDJson { DDD = 9, Region = RegionRequestEnum.Sudeste };

        var validator = new RegisterRegionDDDValidator();

        var validationResult = validator.Validate(request);

        validationResult.Errors.Add(new ValidationFailure("DDD", ErrorsMessages.DDDBetweenTenNinetyNine));

        var mockValidator = new Mock<IValidator<RequestRegionDDDJson>>();

        mockValidator
            .Setup(v => v.Validate(request))
            .Returns(validationResult);

        _mockReadOnlyRepository
            .Setup(r => r.ThereIsDDDNumber(request.DDD))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationErrorsException>(() => _useCase.Execute(request));
        _mockWriteOnlyRepository.Verify(r => r.Add(It.IsAny<RegionDDD>()), Times.Never);
        _mockWorkUnit.Verify(w => w.Commit(), Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldThrowValidationErrorsException_WhenDDDAlreadyExists()
    {
        // Arrange
        var request = new RequestRegionDDDJson { DDD = 11, Region = RegionRequestEnum.Sudeste };

        _mockReadOnlyRepository
            .Setup(r => r.ThereIsDDDNumber(request.DDD))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationErrorsException>(() => _useCase.Execute(request));
        _mockWriteOnlyRepository.Verify(r => r.Add(It.IsAny<RegionDDD>()), Times.Never);
        _mockWorkUnit.Verify(w => w.Commit(), Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldMapAndAddEntityWithUserId_WhenRequestIsValid()
    {
        // Arrange
        var request = new RequestRegionDDDJson { DDD = 11 };

        var entity = new RegionDDD { DDD = 11 };

        var user = new Domain.Entities.User { Id = Guid.NewGuid(), Email = "email@email.com", Name = "McLovin" };

        _mockMapper
            .Setup(m => m.Map<RegionDDD>(request))
            .Returns(entity);

        _mockLoggedUser
            .Setup(l => l.RecoverUser())
            .ReturnsAsync(user);

        _mockReadOnlyRepository
            .Setup(r => r.ThereIsDDDNumber(request.DDD))
            .ReturnsAsync(false);

        // Act
        await _useCase.Execute(request);

        // Assert
        Assert.Equal(user.Id, entity.UserId);
        _mockWriteOnlyRepository.Verify(r => r.Add(entity), Times.Once);
        _mockWorkUnit.Verify(w => w.Commit(), Times.Once);
    }
}
