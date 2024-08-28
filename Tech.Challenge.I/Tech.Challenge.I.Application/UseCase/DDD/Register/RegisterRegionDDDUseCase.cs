using AutoMapper;
using FluentValidation.Results;
using Tech.Challenge.I.Application.Services.LoggedUser;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.DDD;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Application.UseCase.DDD.Register;
public class RegisterRegionDDDUseCase(
    IRegionDDDReadOnlyRepository regionDDDReadOnlyRepository,
    IRegionDDDWriteOnlyRepository regionDDDWriteOnlyRepository,
    IMapper mapper,
    IWorkUnit workUnit,
    ILoggedUser loggedUser) : IRegisterRegionDDDUseCase
{
    private readonly IRegionDDDReadOnlyRepository _regionDDDReadOnlyRepository = regionDDDReadOnlyRepository;
    private readonly IRegionDDDWriteOnlyRepository _regionDDDWriteOnlyRepository = regionDDDWriteOnlyRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IWorkUnit _workUnit = workUnit;
    private readonly ILoggedUser _loggedUser = loggedUser;

    public async Task Execute(RequestRegionDDDJson request)
    {
        await Validate(request);

        var entity = _mapper.Map<Domain.Entities.RegionDDD>(request);

        var loggedUser = await _loggedUser.RecoverUser();

        entity.UserId = loggedUser.Id;

        await _regionDDDWriteOnlyRepository.Add(entity);

        await _workUnit.Commit();
    }

    private async Task Validate(RequestRegionDDDJson request)
    {
        var validator = new RegisterRegionDDDValidator();
        var result = validator.Validate(request);

        var thereIsDDDNumber = await _regionDDDReadOnlyRepository.ThereIsDDDNumber(request.DDD);

        if (thereIsDDDNumber)
            result.Errors.Add(new ValidationFailure("DDD", ErrorsMessages.ThereIsDDDNumber));

        if (!result.IsValid)
        {
            var messageError = result.Errors.Select(error => error.ErrorMessage).Distinct().ToList();
            throw new ValidationErrorsException(messageError);
        }
    }
}
