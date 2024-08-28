using AutoMapper;
using Moq;
using Tech.Challenge.I.Application.Services.Cryptography;
using Tech.Challenge.I.Application.UseCase.User.Register;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.User;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Exceptions.ExceptionBase;
using Tech.Challenge.I.Tests.Mock;

namespace Tech.Challenge.I.Tests.UseCase.User.Register;
public class RegisterUserUseCaseTests
{
    private readonly Mock<IUserReadOnlyRepository> _mockUserReadOnlyRepository;
    private readonly Mock<IUserWriteOnlyRepository> _mockUserWriteOnlyRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IWorkUnit> _mockWorkUnit;
    private readonly MockTokenController _mockTokenController;
    private readonly PasswordEncryptor _passwordEncryptor;
    private readonly RegisterUserUseCase _useCase;
    private const string additionalKey = "123456";
    private const string keyToken = "V1lYOTEyZ0doMVZDc0cyWTY2SFgjVHBVNFozZVpG";

    public RegisterUserUseCaseTests()
    {
        _mockUserReadOnlyRepository = new Mock<IUserReadOnlyRepository>();
        _mockUserWriteOnlyRepository = new Mock<IUserWriteOnlyRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockWorkUnit = new Mock<IWorkUnit>();
        _mockTokenController = new MockTokenController(1000, keyToken);
        _passwordEncryptor = new PasswordEncryptor(additionalKey);

        _useCase = new RegisterUserUseCase(
            _mockUserReadOnlyRepository.Object,
            _mockUserWriteOnlyRepository.Object,
            _mockMapper.Object,
            _mockWorkUnit.Object,
            _passwordEncryptor,
            _mockTokenController);
    }

    [Fact]
    public async Task Execute_ShouldRegisterUserAndReturnToken_WhenRequestIsValid()
    {
        // Arrange
        var request = new RequestRegisterUserJson { Email = "test@example.com", Name = "John McLovin", Password = "password" };

        var user = new Domain.Entities.User { Email = "test@example.com" };

        var encryptedPassword = _passwordEncryptor.Encrypt(request.Password);

        _mockUserReadOnlyRepository
            .Setup(x => x.ThereIsUserWithEmail(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockMapper
            .Setup(x => x.Map<Domain.Entities.User>(request))
            .Returns(user);

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        _mockUserWriteOnlyRepository.Verify(x => x.Add(It.Is<Domain.Entities.User>(u => u.Email == user.Email && u.Password == encryptedPassword)), Times.Once);
        _mockWorkUnit.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowValidationErrorsException_WhenEmailIsAlreadyRegistered()
    {
        // Arrange
        var request = new RequestRegisterUserJson { Email = "test@example.com", Name = "John McLovin", Password = "password" };

        _mockUserReadOnlyRepository
            .Setup(x => x.ThereIsUserWithEmail(request.Email))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationErrorsException>(() => _useCase.Execute(request));
        Assert.Contains(ErrorsMessages.EmailAlreadyRegistered, exception.ErrorMessages);
        _mockUserWriteOnlyRepository.Verify(x => x.Add(It.IsAny<Domain.Entities.User>()), Times.Never);
        _mockWorkUnit.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldThrowValidationErrorsException_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new RequestRegisterUserJson { Email = "", Name = "", Password = "" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationErrorsException>(() => _useCase.Execute(request));
        Assert.Contains(ErrorsMessages.BlankUserName, exception.ErrorMessages);
        Assert.Contains(ErrorsMessages.BlankUserEmail, exception.ErrorMessages);
        Assert.Contains(ErrorsMessages.BlankUserPassword, exception.ErrorMessages);
        _mockUserWriteOnlyRepository.Verify(x => x.Add(It.IsAny<Domain.Entities.User>()), Times.Never);
        _mockWorkUnit.Verify(x => x.Commit(), Times.Never);
    }
}
