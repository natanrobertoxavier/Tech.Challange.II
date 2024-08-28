using Tech.Challenge.I.Communication.Request;

namespace Tech.Challenge.I.Application.UseCase.Contact.Update;
public interface IUpdateContactUseCase
{
    Task Execute(Guid id, RequestContactJson request);
}
