using AutoMapper;
using Tech.Challenge.I.Application.Services.LoggedUser;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.Contact;
using Tech.Challenge.I.Domain.Repositories.DDD;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Application.UseCase.Contact.Register;
public class RegisterContactUseCase(
    IContactReadOnlyRepository contactReadOnlyRepository,
    IRegionDDDReadOnlyRepository regionDDDReadOnlyRepository,
    IContactWriteOnlyRepository contactWriteOnlyRepository,
    IMapper mapper,
    IWorkUnit workUnit,
    ILoggedUser loggedUser) : IRegisterContactUseCase
{
    private readonly IContactReadOnlyRepository _contactReadOnlyRepository = contactReadOnlyRepository;
    private readonly IRegionDDDReadOnlyRepository _regionDDDReadOnlyRepository = regionDDDReadOnlyRepository;
    private readonly IContactWriteOnlyRepository _contactWriteOnlyRepository = contactWriteOnlyRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly ILoggedUser _loggedUser = loggedUser;
    private Guid GuidNull = Guid.Empty;

    public async Task Execute(RequestContactJson request)
    {
        var dddId = await Validate(request);

        var loggedUser = await _loggedUser.RecoverUser();

        var entity = _mapper.Map<Domain.Entities.Contact>(request);
        entity.DDDId = dddId;
        entity.UserId = loggedUser.Id;

        await _contactWriteOnlyRepository.Add(entity);

        await _workUnit.Commit();
    }

    private async Task<Guid> Validate(RequestContactJson request)
    {
        var validator = new RegisterContactValidator();
        var validationResult = validator.Validate(request);

        var regionDDD = await _regionDDDReadOnlyRepository.RecoverListByDDDAsync(request.DDD);

        if (regionDDD is null || !regionDDD.Any())
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("ddd",
                ErrorsMessages.DDDNotFound));

        var thereIsContact = await _contactReadOnlyRepository.ThereIsRegisteredContact(
            regionDDD is not null ? regionDDD.Select(c => c.Id).FirstOrDefault() : GuidNull,
            request.PhoneNumber);

        if (thereIsContact)
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("contact",
                ErrorsMessages.ContactAlreadyRegistered));

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ValidationErrorsException(errorMessages);
        }

        return regionDDD.Select(c => c.Id).FirstOrDefault();
    }
}
