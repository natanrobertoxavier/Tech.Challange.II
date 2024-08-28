using Tech.Challenge.I.Application.Services.Cryptography;
using Tech.Challenge.I.Application.Services.LoggedUser;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.User;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Application.UseCase.User.ChangePassword;
public class ChangePasswordUseCase(
    IUserUpdateOnlyRepository repository,
    ILoggedUser loggedUser,
    PasswordEncryptor passwordEncryptor,
    IWorkUnit workUnit) : IChangePasswordUseCase
{
    private readonly IUserUpdateOnlyRepository _repository = repository;
    private readonly ILoggedUser _loggedUser = loggedUser;
    private readonly PasswordEncryptor _passwordEncryptor = passwordEncryptor;
    private readonly IWorkUnit _workUnit = workUnit;

    public async Task Execute(RequestChangePasswordJson request)
    {
        var loggedUser = await _loggedUser.RecoverUser();

        var user = await _repository.RecoverById(loggedUser.Id);

        Validate(request, user);

        user.Password = _passwordEncryptor.Encrypt(request.NewPassword);

        _repository.Update(user);
        await _workUnit.Commit();
    }

    private void Validate(RequestChangePasswordJson request, Domain.Entities.User user)
    {
        var validator = new ChangePasswordValidator();
        var result = validator.Validate(request);

        var currentPasswordEncrypted = _passwordEncryptor.Encrypt(request.CurrentPassword);

        if (!user.Password.Equals(currentPasswordEncrypted))
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure("currentPassword", ErrorsMessages.InvalidCurrentPassword));
        }

        if (!result.IsValid)
        {
            var mensagens = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ValidationErrorsException(mensagens);
        }
    }
}
