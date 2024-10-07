using Tech.Challenge.I.Domain.Entities;
using Tech.Challenge.I.Infrastructure.RepositoryAccess;
using Tech.Challenge.I.Integration.Tests.Fakes.Entities;

namespace Tech.Challenge.I.Integration.Tests;
public class ContextSeedInMemory
{
    public static (User user, string password) Seed(TechChallengeContext context)
    {
        (var user, string password) = UserBuilder.Build();

        context.Users.Add(user);
        context.SaveChanges();

        return (user, password);
    }
}
