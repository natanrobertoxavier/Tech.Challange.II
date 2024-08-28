namespace Tech.Challenge.I.Application.UseCase.Contact.Delete;
public interface IDeleteContactUseCase
{
    Task<bool> Execute(Guid id);
}
