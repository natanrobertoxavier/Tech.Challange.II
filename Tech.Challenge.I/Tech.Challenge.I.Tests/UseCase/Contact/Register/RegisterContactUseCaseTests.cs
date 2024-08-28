using AutoMapper;
using Moq;
using Tech.Challenge.I.Application.Services.LoggedUser;
using Tech.Challenge.I.Application.UseCase.Contact.Register;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Domain.Entities;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.Contact;
using Tech.Challenge.I.Domain.Repositories.DDD;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Tests.UseCase.Contact.Register;
public class RegisterContactUseCaseTests
{
    private readonly Mock<IContactReadOnlyRepository> _contactReadOnlyRepositoryMock = new();
    private readonly Mock<IRegionDDDReadOnlyRepository> _regionDDDReadOnlyRepositoryMock = new();
    private readonly Mock<IContactWriteOnlyRepository> _contactWriteOnlyRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IWorkUnit> _workUnitMock = new();
    private readonly Mock<ILoggedUser> _loggedUserMock = new();
    private readonly RegisterContactUseCase _useCase;

    public RegisterContactUseCaseTests()
    {
        _useCase = new RegisterContactUseCase(
            _contactReadOnlyRepositoryMock.Object,
            _regionDDDReadOnlyRepositoryMock.Object,
            _contactWriteOnlyRepositoryMock.Object,
            _mapperMock.Object,
            _workUnitMock.Object,
            _loggedUserMock.Object
        );
    }

    [Fact]
    public async Task Execute_ShouldRegisterContact_WhenValidRequest()
    {
        // Arrange
        var request = new RequestContactJson
        {
            FirstName = "John",
            LastName = "McLovin",
            Email = "email@email.com",
            DDD = 11,
            PhoneNumber = "99999-9999"
        };

        var dddList = new List<RegionDDD> { new() { Id = Guid.NewGuid() } };

        var loggedUser = new Domain.Entities.User { Id = Guid.NewGuid() };

        var entity = new Challenge.I.Domain.Entities.Contact
        {
            DDDId = dddList[0].Id,
            FirstName = "John",
            LastName = "McLovin",
            Email = "email@email.com",
            PhoneNumber = "99999-9999",
            UserId = loggedUser.Id,
        };

        _mapperMock
            .Setup(x => x.Map<Challenge.I.Domain.Entities.Contact>(request))
            .Returns(entity);

        _loggedUserMock
            .Setup(x => x.RecoverUser())
            .ReturnsAsync(loggedUser);

        _regionDDDReadOnlyRepositoryMock
            .Setup(x => x.RecoverListByDDDAsync(request.DDD))
            .ReturnsAsync(dddList);

        _contactReadOnlyRepositoryMock
            .Setup(x => x.ThereIsRegisteredContact(It.IsAny<Guid>(), request.PhoneNumber))
            .ReturnsAsync(false);

        // Act
        await _useCase.Execute(request);

        // Assert
        _contactWriteOnlyRepositoryMock.Verify(x => x.Add(It.IsAny<Challenge.I.Domain.Entities.Contact>()), Times.Once);
        _workUnitMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowValidationErrorsException_WhenDDDNotFound()
    {
        // Arrange
        var request = new RequestContactJson { DDD = 11, PhoneNumber = "99999-9999" };

        _regionDDDReadOnlyRepositoryMock
            .Setup(x => x.RecoverListByDDDAsync(request.DDD))
            .ReturnsAsync((IEnumerable<RegionDDD>)null);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationErrorsException>(() => _useCase.Execute(request));

        _contactWriteOnlyRepositoryMock.Verify(x => x.Add(It.IsAny<Challenge.I.Domain.Entities.Contact>()), Times.Never);
        _workUnitMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldThrowValidationErrorsException_WhenContactAlreadyRegistered()
    {
        // Arrange
        var request = new RequestContactJson { DDD = 11, PhoneNumber = "99999-9999" };

        var dddList = new List<RegionDDD> { new() { Id = Guid.NewGuid() } };

        _regionDDDReadOnlyRepositoryMock
            .Setup(x => x.RecoverListByDDDAsync(request.DDD))
            .ReturnsAsync(dddList);

        _contactReadOnlyRepositoryMock
            .Setup(x => x.ThereIsRegisteredContact(It.IsAny<Guid>(), request.PhoneNumber))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationErrorsException>(() => _useCase.Execute(request));

        _contactWriteOnlyRepositoryMock.Verify(x => x.Add(It.IsAny<Challenge.I.Domain.Entities.Contact>()), Times.Never);
        _workUnitMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldThrowValidationErrorsException_WhenRequestValidationFails()
    {
        // Arrange
        var request = new RequestContactJson { DDD = default, PhoneNumber = null };

        _contactReadOnlyRepositoryMock
            .Setup(x => x.ThereIsRegisteredContact(It.IsAny<Guid>(), request.PhoneNumber))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationErrorsException>(() => _useCase.Execute(request));

        _contactWriteOnlyRepositoryMock.Verify(x => x.Add(It.IsAny<Challenge.I.Domain.Entities.Contact>()), Times.Never);
        _workUnitMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldThrowException_WhenCommitFails()
    {
        // Arrange
        var request = new RequestContactJson
        {
            FirstName = "John",
            LastName = "McLovin",
            Email = "email@email.com",
            DDD = 11,
            PhoneNumber = "99999-9999"
        };

        var dddList = new List<RegionDDD> { new() { Id = Guid.NewGuid() } };

        var loggedUser = new Domain.Entities.User { Id = Guid.NewGuid() };

        var entity = new Challenge.I.Domain.Entities.Contact
        {
            DDDId = dddList[0].Id,
            FirstName = "John",
            LastName = "McLovin",
            Email = "email@email.com",
            PhoneNumber = "99999-9999",
            UserId = loggedUser.Id,
        };

        _mapperMock
            .Setup(x => x.Map<Challenge.I.Domain.Entities.Contact>(request))
            .Returns(entity);

        _loggedUserMock
            .Setup(x => x.RecoverUser())
            .ReturnsAsync(loggedUser);

        _regionDDDReadOnlyRepositoryMock
            .Setup(x => x.RecoverListByDDDAsync(request.DDD))
            .ReturnsAsync(dddList);

        _contactReadOnlyRepositoryMock
            .Setup(x => x.ThereIsRegisteredContact(It.IsAny<Guid>(), request.PhoneNumber))
            .ReturnsAsync(false);

        _workUnitMock.Setup(x => x.Commit()).ThrowsAsync(new Exception("Commit failed"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _useCase.Execute(request));

        _contactWriteOnlyRepositoryMock.Verify(x => x.Add(It.IsAny<Challenge.I.Domain.Entities.Contact>()), Times.Once);
    }
}
