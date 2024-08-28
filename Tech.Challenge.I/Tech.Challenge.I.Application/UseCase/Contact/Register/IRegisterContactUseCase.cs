using Tech.Challenge.I.Communication.Request;

namespace Tech.Challenge.I.Application.UseCase.Contact.Register;
public interface IRegisterContactUseCase
{
    Task Execute(RequestContactJson request);
}
