using Tech.Challenge.I.Application.Services.Cryptography;

namespace Tech.Challenge.I.Integration.Tests.Fakes;
public class PasswordEncryptorBuilder
{
    public static PasswordEncryptor Build()
    {
        return new PasswordEncryptor("%xIQ*83Y0K!@");
    }
}
