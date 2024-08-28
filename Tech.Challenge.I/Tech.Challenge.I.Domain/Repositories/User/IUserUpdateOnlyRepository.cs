namespace Tech.Challenge.I.Domain.Repositories.User;
public interface IUserUpdateOnlyRepository
{
    void Update(Entities.User user);
    Task<Entities.User> RecoverById(Guid id);
}
