using AutoMapper;
using Tech.Challenge.I.Application.Services.Cryptography;
using Tech.Challenge.I.Application.Services.Token;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.User;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Application.UseCase.User.Register;

public class RegisterUserUseCase(
    IUserReadOnlyRepository userReadOnlyRepository,
    IUserWriteOnlyRepository userWriteOnlyRepository,
    IMapper mapper,
    IWorkUnit workUnit,
    PasswordEncryptor passwordEncryptor,
    TokenController tokenController) : IRegisterUserUseCase
{
    private readonly IUserReadOnlyRepository _userReadOnlyRepository = userReadOnlyRepository;
    private readonly IUserWriteOnlyRepository _userWriteOnlyRepository = userWriteOnlyRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly PasswordEncryptor _passwordEncryptor = passwordEncryptor;
    private readonly TokenController _tokenController = tokenController;

    public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
    {
        await Validate(request);

        var entity = _mapper.Map<Domain.Entities.User>(request);
        entity.Password = _passwordEncryptor.Encrypt(request.Password);

        await _userWriteOnlyRepository.Add(entity);
        await _workUnit.Commit();

        var token = _tokenController.GenerateToken(entity.Email);

        return new ResponseRegisteredUserJson
        {
            Token = token
        };
    }

    private async Task Validate(RequestRegisterUserJson request)
    {
        var registerUserValidator = new RegisterUserValidator();
        var validationResult = registerUserValidator.Validate(request);

        var thereIsUserWithEmail = await _userReadOnlyRepository.ThereIsUserWithEmail(request.Email);

        if (thereIsUserWithEmail)
        {
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("email", ErrorsMessages.EmailAlreadyRegistered));
        }

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ValidationErrorsException(errorMessages);
        }
    }
}
