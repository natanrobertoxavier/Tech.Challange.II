using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tech.Challenge.I.Api.Controllers.v1;
using Tech.Challenge.I.Application.UseCase.Contact.Delete;
using Tech.Challenge.I.Application.UseCase.Contact.Recover;
using Tech.Challenge.I.Application.UseCase.Contact.Register;
using Tech.Challenge.I.Application.UseCase.Contact.Update;
using Tech.Challenge.I.Communication;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Communication.Request.Enum;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Communication.Response.Enum;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Tests.Controllers;
public class ContactControllerTests
{
    [Fact]
    public async Task RegisterContact_ReturnsCreatedResult_WhenRequestIsValid()
    {
        // Arrange
        var mockUseCase = new Mock<IRegisterContactUseCase>();

        var request = new RequestContactJson
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            DDD = 11,
            PhoneNumber = "99999-9999"
        };

        mockUseCase.Setup(useCase => useCase.Execute(request))
                   .Returns(Task.CompletedTask);

        var controller = new ContactController();

        // Act
        var result = await controller.RegisterContact(mockUseCase.Object, request) as CreatedResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(nameof(ContactController.RecoverAllContacts), result.Location);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task RegisterContact_ReturnsBadRequestResult_WhenRequestIsInvalid()
    {
        // Arrange
        var mockUseCase = new Mock<IRegisterContactUseCase>();

        var request = new RequestContactJson
        {
            FirstName = string.Empty,
            LastName = "Doe",
            Email = string.Empty,
            DDD = 11,
            PhoneNumber = "99999-9999"
        };

        mockUseCase.Setup(useCase => useCase.Execute(request))
                   .ThrowsAsync(new ValidationErrorsException(new List<string>
                   {
                       "Nome em branco",
                       "Email em branco"
                   }));

        var controller = new ContactController();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationErrorsException>(() =>
            controller.RegisterContact(mockUseCase.Object, request));

        // Assert
        Assert.Contains("Nome em branco", exception.ErrorMessages);
        Assert.Contains("Email em branco", exception.ErrorMessages);
    }

    [Fact]
    public async Task RegisterContact_ReturnsUnauthorizedResult_WhenUserIsNotAuthorized()
    {
        // Arrange
        var mockUseCase = new Mock<IRegisterContactUseCase>();

        var request = new RequestContactJson
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            DDD = 11,
            PhoneNumber = "99999-9999"
        };

        mockUseCase.Setup(useCase => useCase.Execute(request))
                   .ThrowsAsync(new UnauthorizedAccessException("Usuário sem permissão"));

        var controller = new ContactController();

        // Act
        var result = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            controller.RegisterContact(mockUseCase.Object, request));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Usuário sem permissão", result.Message);
    }

    [Fact]
    public async Task RecoverAllContacts_ReturnsOkResult_WhenContactsExist()
    {
        // Arrange
        var contactId = Guid.NewGuid();

        var mockUseCase = new Mock<IRecoverContactUseCase>();

        var response = new List<ResponseContactJson>
        {
            new() { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", PhoneNumber = "98888-8888", ContactId = contactId, DDD = 11, Region = RegionResponseEnum.Sudeste.GetDescription() },
            new() { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com", PhoneNumber = "97777-7777", ContactId = contactId, DDD = 11, Region = RegionResponseEnum.Sudeste.GetDescription() }
        };

        mockUseCase.Setup(useCase => useCase.Execute())
                   .ReturnsAsync(response);

        var controller = new ContactController();

        // Act
        var result = await controller.RecoverAllContacts(mockUseCase.Object) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(response, result.Value);
    }

    [Fact]
    public async Task RecoverAllContacts_ReturnsNoContentResult_WhenNoContactsExist()
    {
        // Arrange
        var mockUseCase = new Mock<IRecoverContactUseCase>();

        mockUseCase.Setup(useCase => useCase.Execute())
                   .ReturnsAsync(Enumerable.Empty<ResponseContactJson>());

        var controller = new ContactController();

        // Act
        var result = await controller.RecoverAllContacts(mockUseCase.Object) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact]
    public async Task RecoverAllContacts_ReturnsUnauthorizedResult_WhenUserIsNotAuthorized()
    {
        // Arrange
        var mockUseCase = new Mock<IRecoverContactUseCase>();

        mockUseCase.Setup(useCase => useCase.Execute())
                   .ThrowsAsync(new UnauthorizedAccessException("Usuário sem permissão"));

        var controller = new ContactController();

        // Act
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            controller.RecoverAllContacts(mockUseCase.Object));

        // Assert
        Assert.Equal("Usuário sem permissão", exception.Message);
    }

    [Fact]
    public async Task RecoverContactsByRegion_ReturnsOkResult_WhenContactsExistInRegion()
    {
        // Arrange
        var mockUseCase = new Mock<IRecoverContactUseCase>();
        var region = RegionRequestEnum.Sudeste;

        var response = new List<ResponseContactJson>
        {
            new() { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", PhoneNumber = "98888-8888", ContactId = Guid.NewGuid(), DDD = 11, Region = RegionResponseEnum.Sudeste.GetDescription()},
            new() { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com", PhoneNumber = "97777-7777", ContactId = Guid.NewGuid(), DDD = 13, Region = RegionResponseEnum.Sudeste.GetDescription() }
        };

        mockUseCase.Setup(useCase => useCase.Execute(region))
                   .ReturnsAsync(response);

        var controller = new ContactController();

        // Act
        var result = await controller.RecoverContactsByRegion(region, mockUseCase.Object) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(response, result.Value);
    }

    [Fact]
    public async Task RecoverContactsByRegion_ReturnsNoContentResult_WhenNoContactsExistInRegion()
    {
        // Arrange
        var mockUseCase = new Mock<IRecoverContactUseCase>();
        var region = RegionRequestEnum.Sul;

        mockUseCase.Setup(useCase => useCase.Execute(region))
                   .ReturnsAsync(Enumerable.Empty<ResponseContactJson>());

        var controller = new ContactController();

        // Act
        var result = await controller.RecoverContactsByRegion(region, mockUseCase.Object) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact]
    public async Task RecoverContactsByRegion_ReturnsUnauthorizedResult_WhenUserIsNotAuthorized()
    {
        // Arrange
        var mockUseCase = new Mock<IRecoverContactUseCase>();
        var region = RegionRequestEnum.CentroOeste;

        mockUseCase.Setup(useCase => useCase.Execute(region))
                   .ThrowsAsync(new UnauthorizedAccessException("Usuário sem permissão"));

        var controller = new ContactController();

        // Act
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            controller.RecoverContactsByRegion(region, mockUseCase.Object));

        // Assert
        Assert.Equal("Usuário sem permissão", exception.Message);
    }

    [Fact]
    public async Task RecoverContactsByDDD_ReturnsOkResult_WhenContactsExistForDDD()
    {
        // Arrange
        var contactId = Guid.NewGuid();

        var mockUseCase = new Mock<IRecoverContactUseCase>();
        var ddd = 11;

        var response = new List<ResponseContactJson>
        {
            new() { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", PhoneNumber = "98888-8888", ContactId = contactId, DDD = ddd, Region = RegionResponseEnum.Sudeste.GetDescription() },
            new() { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com", PhoneNumber = "97777-7777", ContactId = contactId, DDD = ddd, Region = RegionResponseEnum.Sudeste.GetDescription() }
        };

        mockUseCase.Setup(useCase => useCase.Execute(ddd))
                   .ReturnsAsync(response);

        var controller = new ContactController();

        // Act
        var result = await controller.RecoverContactsByDDD(ddd, mockUseCase.Object) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(response, result.Value);
    }

    [Fact]
    public async Task RecoverContactsByDDD_ReturnsNoContentResult_WhenNoContactsExistForDDD()
    {
        // Arrange
        var mockUseCase = new Mock<IRecoverContactUseCase>();
        int ddd = 21;

        mockUseCase.Setup(useCase => useCase.Execute(ddd))
                   .ReturnsAsync(Enumerable.Empty<ResponseContactJson>());

        var controller = new ContactController();

        // Act
        var result = await controller.RecoverContactsByDDD(ddd, mockUseCase.Object) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact]
    public async Task RecoverContactsByDDD_ReturnsUnauthorizedResult_WhenUserIsNotAuthorized()
    {
        // Arrange
        var mockUseCase = new Mock<IRecoverContactUseCase>();
        int ddd = 11;

        mockUseCase.Setup(useCase => useCase.Execute(ddd))
                   .ThrowsAsync(new UnauthorizedAccessException("Usuário sem permissão"));

        var controller = new ContactController();

        // Act
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            controller.RecoverContactsByDDD(ddd, mockUseCase.Object));

        // Assert
        Assert.Equal("Usuário sem permissão", exception.Message);
    }

    [Fact]
    public async Task Update_ReturnsOkResult_WhenUpdateIsSuccessful()
    {
        // Arrange
        var mockUseCase = new Mock<IUpdateContactUseCase>();

        var id = Guid.NewGuid();

        var request = new RequestContactJson { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", PhoneNumber = "98888-8888", DDD = 11 };

        mockUseCase.Setup(useCase => useCase.Execute(id, request))
                   .Returns(Task.CompletedTask);

        var controller = new ContactController();

        // Act
        var result = await controller.Update(id, request, mockUseCase.Object) as NoContentResult;

        // Assert
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Remove_ReturnsNoContent_WhenContactIsSuccessfullyRemoved()
    {
        // Arrange
        var mockUseCase = new Mock<IDeleteContactUseCase>();

        Guid validId = Guid.NewGuid();

        mockUseCase.Setup(useCase => useCase.Execute(validId))
                   .ReturnsAsync(true);

        var controller = new ContactController();

        // Act
        var result = await controller.Remove(validId, mockUseCase.Object) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Remove_ReturnsUnprocessableEntity_WhenNoContactIsFound()
    {
        // Arrange
        var mockUseCase = new Mock<IDeleteContactUseCase>();

        Guid nonExistentId = Guid.NewGuid();

        mockUseCase.Setup(useCase => useCase.Execute(nonExistentId))
                   .ReturnsAsync(false);

        var controller = new ContactController();

        // Act
        var result = await controller.Remove(nonExistentId, mockUseCase.Object) as UnprocessableEntityObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
        Assert.Equal(ErrorsMessages.NoContactsFound, result.Value);
    }

    [Fact]
    public async Task Remove_ReturnsBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var mockUseCase = new Mock<IDeleteContactUseCase>();

        Guid invalidId = Guid.Empty;

        mockUseCase.Setup(useCase => useCase.Execute(invalidId))
                   .ReturnsAsync(false);

        var controller = new ContactController();

        // Act
        var result = await controller.Remove(invalidId, mockUseCase.Object) as UnprocessableEntityObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
        Assert.Equal(ErrorsMessages.NoContactsFound, result.Value);
    }
}
