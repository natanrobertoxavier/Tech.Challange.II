using Tech.Challenge.I.Communication;
using Tech.Challenge.I.Communication.Request.Enum;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Domain.Repositories.Contact;
using Tech.Challenge.I.Domain.Repositories.Factories;
using Tech.Challenge.I.Exceptions.ExceptionBase;
using Tech.Challenge.I.Exceptions;

namespace Tech.Challenge.I.Application.UseCase.Contact.Recover;
public class RecoverContactUseCase(
    IContactReadOnlyRepository contactReadOnlyRepository,
    IRegionDDDReadOnlyRepositoryFactory repositoryFactory) : IRecoverContactUseCase
{
    private readonly IContactReadOnlyRepository _contactReadOnlyRepository = contactReadOnlyRepository;
    private readonly IRegionDDDReadOnlyRepositoryFactory _repositoryFactory = repositoryFactory;

    public async Task<IEnumerable<ResponseContactJson>> Execute()
    {
        var entities = await _contactReadOnlyRepository.RecoverAllAsync();

        return await MapToResponseContactJson(entities);
    }

    public async Task<IEnumerable<ResponseContactJson>> Execute(RegionRequestEnum region)
    {
        var dddIds = await RecoverDDDIdsByRegion(region.GetDescription());

        var entities = await _contactReadOnlyRepository.RecoverAllByDDDIdAsync(dddIds);

        return await MapToResponseContactJson(entities);
    }

    public async Task<IEnumerable<ResponseContactJson>> Execute(int ddd)
    {
        var regionIds = await RecoverRegionIdByDDD(ddd);

        var entities = await _contactReadOnlyRepository.RecoverByDDDIdAsync(regionIds);

        if (entities is not null &&
            entities.Any())
            return await MapToResponseContactJson(entities);

        return new List<ResponseContactJson>();
    }

    private async Task<IEnumerable<ResponseContactJson>> MapToResponseContactJson(IEnumerable<Domain.Entities.Contact> entities)
    {
        var tasks = entities.Select(async entity =>
        {
            var (regionReadOnlyrepository, scope) = _repositoryFactory.Create();

            using (scope)
            {
                var ddd = await regionReadOnlyrepository.RecoverByIdAsync(entity.DDDId);

                return new ResponseContactJson
                {
                    ContactId = entity.Id,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    DDD = ddd.DDD,
                    Region = ddd.Region,
                    Email = entity.Email,
                    PhoneNumber = entity.PhoneNumber
                };
            }
        });

        var responseContactJson = await Task.WhenAll(tasks);
        return responseContactJson;
    }

    private async Task<IEnumerable<Guid>> RecoverDDDIdsByRegion(string region)
    {
        var (regionReadOnlyRepository, scope) = _repositoryFactory.Create();

        using (scope)
        {
            var ddd = await regionReadOnlyRepository.RecoverListDDDByRegionAsync(region);

            return ddd.Select(ddd => ddd.Id).ToList();
        }
    }

    private async Task<Guid> RecoverRegionIdByDDD(int ddd)
    {
        var (regionReadOnlyRepository, scope) = _repositoryFactory.Create();

        using (scope)
        {
            var regionDDD = await regionReadOnlyRepository.RecoverByDDDAsync(ddd) ??
                throw new ValidationErrorsException(new List<string>() { ErrorsMessages.DDDNotFound });

            return regionDDD.Id;
        }
    }
}
