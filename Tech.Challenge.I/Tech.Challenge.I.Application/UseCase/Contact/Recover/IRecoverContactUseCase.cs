using Tech.Challenge.I.Communication.Request.Enum;
using Tech.Challenge.I.Communication.Response;

namespace Tech.Challenge.I.Application.UseCase.Contact.Recover;
public interface IRecoverContactUseCase
{
    Task<IEnumerable<ResponseContactJson>> Execute(int pageNumber, int pageSize);
    Task<IEnumerable<ResponseContactJson>> Execute(RegionRequestEnum region, int pageNumber, int pageSize);
    Task<IEnumerable<ResponseContactJson>> Execute(int ddd, int pageNumber, int pageSize);
}
