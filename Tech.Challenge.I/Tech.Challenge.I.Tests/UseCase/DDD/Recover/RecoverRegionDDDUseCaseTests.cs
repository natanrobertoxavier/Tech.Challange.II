using AutoMapper;
using Moq;
using Tech.Challenge.I.Application.UseCase.DDD.Recover;
using Tech.Challenge.I.Communication;
using Tech.Challenge.I.Communication.Request.Enum;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Communication.Response.Enum;
using Tech.Challenge.I.Domain.Entities;
using Tech.Challenge.I.Domain.Repositories.DDD;

namespace Tech.Challenge.I.Tests.UseCase.DDD.Recover;
public class RecoverRegionDDDUseCaseTests
{
    private readonly Mock<IRegionDDDReadOnlyRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly RecoverRegionDDDUseCase _useCase;

    public RecoverRegionDDDUseCaseTests()
    {
        _mockRepository = new Mock<IRegionDDDReadOnlyRepository>();
        _mockMapper = new Mock<IMapper>();
        _useCase = new RecoverRegionDDDUseCase(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Execute_ShouldReturnMappedResult_WhenRecoverAllIsCalled()
    {
        // Arrange
        var dddList = new List<RegionDDD>
        {
            new() { Id = Guid.NewGuid(), DDD = 11, Region = RegionResponseEnum.Sudeste.GetDescription() },
            new() { Id = Guid.NewGuid(), DDD = 14, Region = RegionResponseEnum.Sudeste.GetDescription() },
        };

        var mappedResult = new List<ResponseRegionDDDJson>
        {
            new() { DDD = 11, Region = RegionResponseEnum.Sudeste.GetDescription() },
            new() { DDD = 14, Region = RegionResponseEnum.Sudeste.GetDescription() },
        };

        _mockRepository
            .Setup(repo => repo.RecoverAllAsync())
            .ReturnsAsync(dddList);

        _mockMapper
            .Setup(mapper => mapper.Map<IEnumerable<ResponseRegionDDDJson>>(dddList))
            .Returns(mappedResult);

        // Act
        var result = await _useCase.Execute();

        // Assert
        Assert.Equal(mappedResult, result);
        _mockRepository.Verify(repo => repo.RecoverAllAsync(), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<IEnumerable<ResponseRegionDDDJson>>(dddList), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldReturnMappedResult_WhenRecoverListDDDByRegionIsCalled()
    {
        // Arrange
        var request = RegionRequestEnum.Sul;

        var dddList = new List<RegionDDD>
        {
            new() { Id = Guid.NewGuid(), DDD = 41, Region = RegionResponseEnum.Sul.GetDescription() },
            new() { Id = Guid.NewGuid(), DDD = 42, Region = RegionResponseEnum.Sul.GetDescription() },
        };

        var mappedResult = new List<ResponseRegionDDDJson>
        {
            new() { DDD = 41, Region = RegionResponseEnum.Sul.GetDescription() },
            new() { DDD = 42, Region = RegionResponseEnum.Sul.GetDescription() },
        };
        _mockRepository
            .Setup(repo => repo.RecoverListDDDByRegionAsync(request.GetDescription()))
            .ReturnsAsync(dddList);

        _mockMapper
            .Setup(mapper => mapper.Map<IEnumerable<ResponseRegionDDDJson>>(dddList))
            .Returns(mappedResult);

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.Equal(mappedResult, result);
        _mockRepository.Verify(repo => repo.RecoverListDDDByRegionAsync(request.GetDescription()), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<IEnumerable<ResponseRegionDDDJson>>(dddList), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenNoDataIsFound()
    {
        // Arrange
        _mockRepository
            .Setup(repo => repo.RecoverAllAsync())
            .ReturnsAsync(new List<RegionDDD>());

        _mockMapper
            .Setup(mapper => mapper.Map<IEnumerable<ResponseRegionDDDJson>>(It.IsAny<IEnumerable<RegionDDD>>()))
            .Returns(new List<ResponseRegionDDDJson>());

        // Act
        var result = await _useCase.Execute();

        // Assert
        Assert.Empty(result);
        _mockRepository.Verify(repo => repo.RecoverAllAsync(), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<IEnumerable<ResponseRegionDDDJson>>(It.IsAny<IEnumerable<RegionDDD>>()), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenRecoverListDDDByRegionReturnsEmpty()
    {
        // Arrange
        var request = RegionRequestEnum.Nordeste;

        _mockRepository
            .Setup(repo => repo.RecoverListDDDByRegionAsync(request.GetDescription()))
            .ReturnsAsync(new List<RegionDDD>());

        _mockMapper
            .Setup(mapper => mapper.Map<IEnumerable<ResponseRegionDDDJson>>(It.IsAny<IEnumerable<RegionDDD>>()))
            .Returns(new List<ResponseRegionDDDJson>());

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.Empty(result);
        _mockRepository.Verify(repo => repo.RecoverListDDDByRegionAsync(request.GetDescription()), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<IEnumerable<ResponseRegionDDDJson>>(It.IsAny<IEnumerable<RegionDDD>>()), Times.Once);
    }
}
