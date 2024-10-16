﻿using AutoMapper;
using Moq;
using Tech.Challenge.I.Application.UseCase.Contact.Update;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Domain.Entities;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.Contact;
using Tech.Challenge.I.Domain.Repositories.DDD;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Tests.UseCase.Contact.Update;
public class UpdateContactUseCaseTests
{
    private readonly Mock<IRegionDDDReadOnlyRepository> _regionDDDReadOnlyRepositoryMock = new();
    private readonly Mock<IContactWriteOnlyRepository> _contactWriteOnlyRepositoryMock = new();
    private readonly Mock<IWorkUnit> _workUnitMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly UpdateContactUseCase _useCase;

    public UpdateContactUseCaseTests()
    {
        _useCase = new UpdateContactUseCase(
            _regionDDDReadOnlyRepositoryMock.Object,
            _contactWriteOnlyRepositoryMock.Object,
            _workUnitMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Execute_ShouldUpdateContact_WhenValidRequestAndDDD()
    {
        // Arrange
        _regionDDDReadOnlyRepositoryMock.Setup(x => x.RecoverListByDDDAsync(11))
            .ReturnsAsync(
            new List<RegionDDD>()
            { 
                new()
                {
                    Id = Guid.NewGuid(),
                    DDD = 11,
                    Region = "Sudeste",
                    RegistrationDate = DateTime.Now,
                    UserId = Guid.NewGuid()
                }
            });

        var id = Guid.NewGuid();

        var request = new RequestContactJson
        {
            DDD = 11,
            PhoneNumber = "99999-9999",
            Email = "email@email.com",
            FirstName = "John",
            LastName = "Cena"
        };

        var contactToUpdate = new Domain.Entities.Contact();

        var ddd = new RegionDDD { Id = Guid.NewGuid() };

        _mapperMock
            .Setup(x => x.Map<Domain.Entities.Contact>(request))
            .Returns(contactToUpdate);

        _regionDDDReadOnlyRepositoryMock
            .Setup(x => x.RecoverByDDDAsync(request.DDD))
            .ReturnsAsync(ddd);

        // Act
        await _useCase.Execute(id, request);

        // Assert
        Assert.Equal(id, contactToUpdate.Id);
        Assert.Equal(ddd.Id, contactToUpdate.DDDId);
        _contactWriteOnlyRepositoryMock.Verify(x => x.Update(contactToUpdate), Times.Once);
        _workUnitMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldThrowException_WhenDDDNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new RequestContactJson { DDD = 11, PhoneNumber = "99999-9999" };

        _regionDDDReadOnlyRepositoryMock
            .Setup(x => x.RecoverByDDDAsync(request.DDD))
            .ReturnsAsync((RegionDDD)null);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationErrorsException>(() => _useCase.Execute(id, request));

        _contactWriteOnlyRepositoryMock.Verify(x => x.Update(It.IsAny<Domain.Entities.Contact>()), Times.Never);
        _workUnitMock.Verify(x => x.Commit(), Times.Never);
    }


    [Fact]
    public async Task Execute_ShouldThrowException_WhenValidateFails()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new RequestContactJson { DDD = 11, PhoneNumber = "99999-9999" };

        var contactToUpdate = new Domain.Entities.Contact();

        var ddd = new RegionDDD { Id = Guid.NewGuid() };

        _mapperMock
            .Setup(x => x.Map<Domain.Entities.Contact>(request))
            .Returns(contactToUpdate);

        _regionDDDReadOnlyRepositoryMock
            .Setup(x => x.RecoverByDDDAsync(request.DDD))
            .ReturnsAsync(ddd);

        _workUnitMock
            .Setup(x => x.Commit())
            .ThrowsAsync(new Exception("Commit failed"));

        // Act
        var result = await Assert.ThrowsAsync<ValidationErrorsException>(() => _useCase.Execute(id, request));

        //Assert
        result.ErrorMessages.Contains(ErrorsMessages.BlankFirstName);
        result.ErrorMessages.Contains(ErrorsMessages.BlankLastName);
        result.ErrorMessages.Contains(ErrorsMessages.BlankEmail);
        result.ErrorMessages.Contains(ErrorsMessages.DDDNotFound);
    }
}
