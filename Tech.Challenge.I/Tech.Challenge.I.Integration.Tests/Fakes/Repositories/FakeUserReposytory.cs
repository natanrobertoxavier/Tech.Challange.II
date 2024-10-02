using Microsoft.EntityFrameworkCore;
using Tech.Challenge.I.Domain.Entities;
using Tech.Challenge.I.Domain.Repositories.User;
using Tech.Challenge.I.Infrastructure.RepositoryAccess;

namespace Tech.Challenge.I.Integration.Tests.Fakes.Repositories;
public class FakeUserReposytory(
    TechChallengeContext context) : IUserReadOnlyRepository,
                                    IUserWriteOnlyRepository,
                                    IUserUpdateOnlyRepository
{
    private readonly TechChallengeContext _context = context;

    public async Task Add(User user) =>
        await _context.Users.AddAsync(user);

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

    public async Task<bool> ThereIsUserWithEmail(string email) =>
        await _context.Users.AnyAsync(c => c.Email.Equals(email));

    public void Update(User user)
    {
        throw new NotImplementedException();
    }
}
