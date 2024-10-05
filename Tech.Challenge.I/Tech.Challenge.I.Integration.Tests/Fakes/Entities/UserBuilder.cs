using Tech.Challenge.I.Domain.Entities;

namespace Tech.Challenge.I.Integration.Tests.Fakes.Entities;
public class UserBuilder
{
    public static (User user, string password) Build()
    {
        var password = "123456";
        var id = Guid.NewGuid();

        var builtUser = new User()
        {
            Id = id,
            Name = "John Cena",
            Email = $"{id.ToString()[..6]}@email.com",
            Password = PasswordEncryptorBuilder.Build().Encrypt(password)
        };

        return (builtUser, password);
    }
}
