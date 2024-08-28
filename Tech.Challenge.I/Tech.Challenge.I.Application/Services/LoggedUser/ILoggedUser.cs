namespace Tech.Challenge.I.Application.Services.LoggedUser;
public interface ILoggedUser
{
    Task<Domain.Entities.User> RecoverUser();
}
