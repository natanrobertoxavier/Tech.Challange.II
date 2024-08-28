using Moq;
using Tech.Challenge.I.Application.Services.Cryptography;
using Tech.Challenge.I.Application.Services.LoggedUser;
using Tech.Challenge.I.Application.UseCase.User.ChangePassword;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.User;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Tests.UseCase.User.ChangePassword;
public class ChangePasswordUseCaseTests
{
    private readonly Mock<IUserUpdateOnlyRepository> _mockRepository;
    private readonly Mock<ILoggedUser> _mockLoggedUser;
    private readonly Mock<IWorkUnit> _mockWorkUnit;
    private readonly PasswordEncryptor _passwordEncryptor;
    private readonly ChangePasswordUseCase _useCase;
    private const string additionalKey = "%xIQ*83Y0K!@";

    public ChangePasswordUseCaseTests()
    {
        _mockRepository = new Mock<IUserUpdateOnlyRepository>();
        _mockLoggedUser = new Mock<ILoggedUser>();
        _passwordEncryptor = new PasswordEncryptor(additionalKey);
        _mockWorkUnit = new Mock<IWorkUnit>();

        _useCase = new ChangePasswordUseCase(
            _mockRepository.Object,
            _mockLoggedUser.Object,
            _passwordEncryptor,
            _mockWorkUnit.Object);
    }

    [Fact]
    public async Task Execute_ShouldUpdatePasswordAndCommit_WhenRequestIsValid()
    {
        // Arrange
        var encryptedCurrentPass = _passwordEncryptor.Encrypt("encryptedCurrentPass");
        var encryptedNewPass = _passwordEncryptor.Encrypt("newPass");

        var request = new RequestChangePasswordJson { CurrentPassword = "encryptedCurrentPass", NewPassword = "newPass" };
        var loggedUser = new Domain.Entities.User { Id = Guid.NewGuid(), Password = encryptedCurrentPass };

        _mockLoggedUser
            .Setup(x => x.RecoverUser())
            .ReturnsAsync(loggedUser);

        _mockRepository
            .Setup(x => x.RecoverById(loggedUser.Id))
            .ReturnsAsync(loggedUser);

        // Act
        await _useCase.Execute(request);

        // Assert
        _mockRepository.Verify(x => x.Update(It.Is<Domain.Entities.User>(u => u.Password == encryptedNewPass)), Times.Once);
        _mockWorkUnit.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowValidationErrorsException_WhenCurrentPasswordIsInvalid()
    {
        // Arrange
        var request = new RequestChangePasswordJson { CurrentPassword = "wrongPass", NewPassword = "newPass" };
        var loggedUser = new Domain.Entities.User { Id = Guid.NewGuid(), Password = "encryptedCurrentPass" };

        _mockLoggedUser
            .Setup(x => x.RecoverUser())
            .ReturnsAsync(loggedUser);

        _mockRepository
            .Setup(x => x.RecoverById(loggedUser.Id))
            .ReturnsAsync(loggedUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationErrorsException>(() => _useCase.Execute(request));
        Assert.Contains(ErrorsMessages.InvalidCurrentPassword, exception.ErrorMessages);
        _mockRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
        _mockWorkUnit.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task Execute_ShouldThrowValidationErrorsException_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new RequestChangePasswordJson { CurrentPassword = "", NewPassword = "" };
        var loggedUser = new Domain.Entities.User { Id = Guid.NewGuid(), Password = "encryptedCurrentPass" };

        _mockLoggedUser
            .Setup(x => x.RecoverUser())
            .ReturnsAsync(loggedUser);

        _mockRepository
            .Setup(x => x.RecoverById(loggedUser.Id))
            .ReturnsAsync(loggedUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationErrorsException>(() => _useCase.Execute(request));
        Assert.Contains(ErrorsMessages.BlankUserPassword, exception.ErrorMessages);
        Assert.Contains(ErrorsMessages.InvalidCurrentPassword, exception.ErrorMessages);
        _mockRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.User>()), Times.Never);
        _mockWorkUnit.Verify(x => x.Commit(), Times.Never);
    }
}
