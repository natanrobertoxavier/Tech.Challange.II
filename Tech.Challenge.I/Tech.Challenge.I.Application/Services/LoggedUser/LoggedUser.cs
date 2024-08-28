using Microsoft.AspNetCore.Http;
using Tech.Challenge.I.Application.Services.Token;
using Tech.Challenge.I.Domain.Entities;
using Tech.Challenge.I.Domain.Repositories.User;

namespace Tech.Challenge.I.Application.Services.LoggedUser;
public class LoggedUser(
    IHttpContextAccessor httpContextAccessor,
    TokenController tokenController,
    IUserReadOnlyRepository repository) : ILoggedUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly TokenController _tokenController = tokenController;
    private readonly IUserReadOnlyRepository _repository = repository;

    public async Task<User> RecoverUser()
    {
        var authorization = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

        var token = authorization["Bearer".Length..].Trim();

        var emailUsuario = _tokenController.RecoverEmail(token);

        var usuario = await _repository.RecoverByEmailAsync(emailUsuario);

        return usuario;
    }
}
