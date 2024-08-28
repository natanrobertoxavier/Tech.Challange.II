using Moq;
using Tech.Challenge.I.Application.Services.Cryptography;
using Tech.Challenge.I.Application.UseCase.User.Login;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Domain.Repositories.User;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Exceptions.ExceptionBase;
using Tech.Challenge.I.Tests.Mock;

namespace Tech.Challenge.I.Tests.UseCase.User.Login;
public class LoginUseCaseTests
{
    private readonly Mock<IUserReadOnlyRepository> _mockUserReadOnlyRepository;
    private readonly MockTokenController _mockTokenController;
    private readonly PasswordEncryptor _passwordEncryptor;
    private readonly LoginUseCase _useCase;
    private const string additionalKey = "123456";
    private const string keyToken = "V1lYOTEyZ0doMVZDc0cyWTY2SFgjVHBVNFozZVpG";

    public LoginUseCaseTests()
    {
        _mockUserReadOnlyRepository = new Mock<IUserReadOnlyRepository>();
        _mockTokenController = new MockTokenController(1000, keyToken);
        _passwordEncryptor = new PasswordEncryptor(additionalKey);

        _useCase = new LoginUseCase(
            _mockUserReadOnlyRepository.Object,
            _passwordEncryptor,
            _mockTokenController
        );
    }

    [Fact]
    public async Task Execute_ShouldReturnResponseLoginJson_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new RequestLoginJson { Email = "email@example.com", Password = "password123" };

        var encryptedPassword = _passwordEncryptor.Encrypt(request.Password);

        var user = new Domain.Entities.User { Name = "John McLovin", Email = "user@example.com" };

        _mockUserReadOnlyRepository
            .Setup(r => r.RecoverEmailPasswordAsync(request.Email, encryptedPassword))
            .ReturnsAsync(user);

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.Equal(user.Name, result.Name);
        _mockUserReadOnlyRepository.Verify(r => r.RecoverEmailPasswordAsync(request.Email, encryptedPassword), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowInvalidLoginException_WhenCredentialsAreInvalid()
    {
        // Arrange
        var request = new RequestLoginJson { Email = "email@example.com", Password = "invalidpassword" };

        var encryptedPassword = _passwordEncryptor.Encrypt(request.Password);

        _mockUserReadOnlyRepository
            .Setup(r => r.RecoverEmailPasswordAsync(request.Email, encryptedPassword))
            .ReturnsAsync((Domain.Entities.User)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidLoginException>(() => _useCase.Execute(request));

        Assert.NotNull(exception);
        Assert.Equal(ErrorsMessages.InvalidLogin, exception.Message);
        _mockUserReadOnlyRepository.Verify(r => r.RecoverEmailPasswordAsync(request.Email, encryptedPassword), Times.Once);
    }
}
