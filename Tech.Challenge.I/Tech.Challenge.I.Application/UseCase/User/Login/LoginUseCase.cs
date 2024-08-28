using Tech.Challenge.I.Application.Services.Cryptography;
using Tech.Challenge.I.Application.Services.Token;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Domain.Repositories.User;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Application.UseCase.User.Login;
public class LoginUseCase(
    IUserReadOnlyRepository userReadOnlyRepository,
    PasswordEncryptor passwordEncryptor,
    TokenController tokenController) : ILoginUseCase
{
    private readonly IUserReadOnlyRepository _userReadOnlyRepository = userReadOnlyRepository;
    private readonly PasswordEncryptor _passwordEncryptor = passwordEncryptor;
    private readonly TokenController _tokenController = tokenController;

    public async Task<ResponseLoginJson> Execute(RequestLoginJson request)
    {
        var encryptedPassword = _passwordEncryptor.Encrypt(request.Password);

        var user = await _userReadOnlyRepository.RecoverEmailPasswordAsync(request.Email, encryptedPassword) ??
            throw new InvalidLoginException();

        return new ResponseLoginJson
        {
            Name = user.Name,
            Token = _tokenController.GenerateToken(user.Email)
        };
    }
}
