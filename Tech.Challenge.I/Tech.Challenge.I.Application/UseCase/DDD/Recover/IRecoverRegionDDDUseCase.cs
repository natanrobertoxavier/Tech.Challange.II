using Tech.Challenge.I.Communication.Request.Enum;
using Tech.Challenge.I.Communication.Response;

namespace Tech.Challenge.I.Application.UseCase.DDD.Recover;
public interface IRecoverRegionDDDUseCase
{
    Task<IEnumerable<ResponseRegionDDDJson>> Execute();
    Task<IEnumerable<ResponseRegionDDDJson>> Execute(RegionRequestEnum request);
}
