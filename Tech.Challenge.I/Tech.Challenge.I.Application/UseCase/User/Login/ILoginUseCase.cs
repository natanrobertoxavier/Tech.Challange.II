using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Communication.Response;

namespace Tech.Challenge.I.Application.UseCase.User.Login;
public interface ILoginUseCase
{
    Task<ResponseLoginJson> Execute(RequestLoginJson request);
}
