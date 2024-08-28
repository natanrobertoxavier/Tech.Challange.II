using Tech.Challenge.I.Communication.Request;

namespace Tech.Challenge.I.Application.UseCase.DDD.Register;
public interface IRegisterRegionDDDUseCase
{
    Task Execute(RequestRegionDDDJson request);
}
