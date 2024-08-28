namespace Tech.Challenge.I.Domain.Repositories.User;

public interface IUserReadOnlyRepository
{
    Task<bool> ThereIsUserWithEmail(string email);
    Task<Entities.User> RecoverByEmailPasswordAsync(string email, string senha);
    Task<Entities.User> RecoverByEmailAsync(string email);
    Task<Entities.User> RecoverEmailPasswordAsync(string email, string encryptedPassword);
}
