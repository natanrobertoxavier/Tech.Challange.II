using Tech.Challenge.I.Communication.Request;

namespace Tech.Challenge.I.Application.UseCase.User.ChangePassword;
public interface IChangePasswordUseCase
{
    Task Execute(RequestChangePasswordJson requisicao);
}
