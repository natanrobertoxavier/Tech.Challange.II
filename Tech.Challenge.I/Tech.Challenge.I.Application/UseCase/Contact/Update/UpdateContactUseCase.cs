using AutoMapper;
using Tech.Challenge.I.Application.UseCase.Contact.Register;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.Contact;
using Tech.Challenge.I.Domain.Repositories.DDD;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Application.UseCase.Contact.Update;
public class UpdateContactUseCase(
    IRegionDDDReadOnlyRepository regionReadOnlyRepository,
    IContactWriteOnlyRepository contactWriteOnlyRepository,
    IWorkUnit workUnit,
    IMapper mapper) : IUpdateContactUseCase
{
    private readonly IContactWriteOnlyRepository _contactWriteOnlyRepository = contactWriteOnlyRepository;
    private readonly IRegionDDDReadOnlyRepository _regionReadOnlyRepository = regionReadOnlyRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IWorkUnit _workUnit = workUnit;

    public async Task Execute(Guid id, RequestContactJson request)
    {
        await Validate(request);

        var contactToUpdate = _mapper.Map<Domain.Entities.Contact>(request);

        var ddd = await _regionReadOnlyRepository.RecoverByDDDAsync(request.DDD) ??
            throw new ValidationErrorsException(new List<string>() { ErrorsMessages.DDDNotFound });

        contactToUpdate.Id = id;
        contactToUpdate.DDDId = ddd.Id;

        _contactWriteOnlyRepository.Update(contactToUpdate);
        await _workUnit.Commit();
    }

    private async Task Validate(RequestContactJson request)
    {
        var validator = new RegisterContactValidator();
        var validationResult = validator.Validate(request);

        var regionDDD = await _regionReadOnlyRepository.RecoverListByDDDAsync(request.DDD);

        if (regionDDD is null || !regionDDD.Any())
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("ddd",
                ErrorsMessages.DDDNotFound));

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ValidationErrorsException(errorMessages);
        }
    }
}
