using AutoMapper;
using Tech.Challenge.I.Communication;
using Tech.Challenge.I.Communication.Request.Enum;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Domain.Repositories.DDD;

namespace Tech.Challenge.I.Application.UseCase.DDD.Recover;
public class RecoverRegionDDDUseCase(
    IRegionDDDReadOnlyRepository regionDDDReadOnlyRepository,
    IMapper mapper) : IRecoverRegionDDDUseCase
{
    private readonly IRegionDDDReadOnlyRepository _regionDDDReadOnlyRepository = regionDDDReadOnlyRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<ResponseRegionDDDJson>> Execute()
    {
        var result = await _regionDDDReadOnlyRepository.RecoverAllAsync();

        return _mapper.Map<IEnumerable<ResponseRegionDDDJson>>(result);
    }

    public async Task<IEnumerable<ResponseRegionDDDJson>> Execute(RegionRequestEnum request)
    {
        var result = await _regionDDDReadOnlyRepository.RecoverListDDDByRegionAsync(request.GetDescription());

        return _mapper.Map<IEnumerable<ResponseRegionDDDJson>>(result);
    }
}
