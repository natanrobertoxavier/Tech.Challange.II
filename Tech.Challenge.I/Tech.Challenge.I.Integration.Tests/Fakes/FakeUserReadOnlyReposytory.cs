using Tech.Challenge.I.Domain.Entities;
using Tech.Challenge.I.Domain.Repositories.User;

namespace Tech.Challenge.I.Integration.Tests.Fakes;
public class FakeUserReadOnlyReposytory : IUserReadOnlyRepository, IUserWriteOnlyRepository, IUserUpdateOnlyRepository
{
    public Task Add(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User> RecoverByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User> RecoverById(Guid id)
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

    public void Update(User user)
    {
        throw new NotImplementedException();
    }
}
