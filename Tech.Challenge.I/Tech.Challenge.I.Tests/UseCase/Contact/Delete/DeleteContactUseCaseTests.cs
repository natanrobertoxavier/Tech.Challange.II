using Moq;
using Tech.Challenge.I.Application.UseCase.Contact.Delete;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.Contact;

namespace Tech.Challenge.I.Tests.UseCase.Contact.Delete;

public class DeleteContactUseCaseTests
{
    private readonly Mock<IContactReadOnlyRepository> _mockReadOnlyRepository;
    private readonly Mock<IContactWriteOnlyRepository> _mockWriteOnlyRepository;
    private readonly Mock<IWorkUnit> _mockWorkUnit;
    private readonly DeleteContactUseCase _useCase;

    public DeleteContactUseCaseTests()
    {
        _mockReadOnlyRepository = new Mock<IContactReadOnlyRepository>();
        _mockWriteOnlyRepository = new Mock<IContactWriteOnlyRepository>();
        _mockWorkUnit = new Mock<IWorkUnit>();

        _useCase = new DeleteContactUseCase(
            _mockReadOnlyRepository.Object,
            _mockWriteOnlyRepository.Object,
            _mockWorkUnit.Object);
    }

    [Fact]
    public async Task Execute_ReturnsTrue_WhenContactIsSuccessfullyRemoved()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contact = new Challenge.I.Domain.Entities.Contact();

        _mockReadOnlyRepository.Setup(repo => repo.RecoverByContactIdAsync(contactId))
                               .ReturnsAsync(contact);

        // Act
        var result = await _useCase.Execute(contactId);

        // Assert
        Assert.True(result);
        _mockWriteOnlyRepository.Verify(repo => repo.Remove(contact), Times.Once);
        _mockWorkUnit.Verify(uow => uow.Commit(), Times.Once);
    }

    [Fact]
    public async Task Execute_ReturnsFalse_WhenContactIsNotFound()
    {
        // Arrange
        var contactId = Guid.NewGuid();

        _mockReadOnlyRepository.Setup(repo => repo.RecoverByContactIdAsync(contactId))
                               .ReturnsAsync((Challenge.I.Domain.Entities.Contact)null);

        // Act
        var result = await _useCase.Execute(contactId);

        // Assert
        Assert.False(result);
        _mockWriteOnlyRepository.Verify(repo => repo.Remove(It.IsAny<Challenge.I.Domain.Entities.Contact>()), Times.Never);
        _mockWorkUnit.Verify(uow => uow.Commit(), Times.Never);
    }

    [Fact]
    public async Task Execute_ThrowsException_WhenCommitFails()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var contact = new Challenge.I.Domain.Entities.Contact();

        _mockReadOnlyRepository.Setup(repo => repo.RecoverByContactIdAsync(contactId))
                               .ReturnsAsync(contact);

        _mockWriteOnlyRepository.Setup(repo => repo.Remove(contact));

        _mockWorkUnit.Setup(uow => uow.Commit())
                     .ThrowsAsync(new Exception("Commit failed"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _useCase.Execute(contactId));
        _mockWriteOnlyRepository.Verify(repo => repo.Remove(contact), Times.Once);
        _mockWorkUnit.Verify(uow => uow.Commit(), Times.Once);
    }
}
