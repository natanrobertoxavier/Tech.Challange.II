using Tech.Challenge.I.Domain.Entities;
using Tech.Challenge.I.Domain.Repositories.User;

namespace Tech.Challenge.I.Integration.Tests.Fakes;
public class FakeUserReadOnlyReposytory : IUserReadOnlyRepository
{
    public Task<User> RecoverByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User> RecoverByEmailPasswordAsync(string email, string senha)
    {
        throw new NotImplementedException();
    }

    public Task<User> RecoverEmailPasswordAsync(string email, string encryptedPassword)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ThereIsUserWithEmail(string email)
    {
        throw new NotImplementedException();
    }
}
