using Microsoft.EntityFrameworkCore;
using Tech.Challenge.I.Domain.Entities;
using Tech.Challenge.I.Domain.Repositories.User;

namespace Tech.Challenge.I.Infrastructure.RepositoryAccess.Repository;

public class UserRepository(
    TechChallengeContext context) : IUserReadOnlyRepository,
                                    IUserWriteOnlyRepository,
                                    IUserUpdateOnlyRepository
{
    private readonly TechChallengeContext _context = context;

#pragma warning disable CS8603 // Possível retorno de referência nula.
    public async Task<User> RecoverEmailPasswordAsync(string email, string password) =>
        await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email.Equals(email) &&
                                 c.Password.Equals(password));
    public async Task<bool> ThereIsUserWithEmail(string email) =>
        await _context.Users.AnyAsync(c => c.Email.Equals(email));

    public async Task<User> RecoverByEmailAsync(string email) =>
       await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email.Equals(email));

    public Task<User> RecoverByEmailPasswordAsync(string email, string senha)
    {
        throw new NotImplementedException();
    }

    public async Task Add(User user) =>
        await _context.Users.AddAsync(user);

    public void Update(User user) =>
        _context.Users.Update(user);

    public async Task<User> RecoverById(Guid id) => await _context.Users.
            FirstOrDefaultAsync(c => c.Id == id);
#pragma warning restore CS8603 // Possível retorno de referência nula.
}
