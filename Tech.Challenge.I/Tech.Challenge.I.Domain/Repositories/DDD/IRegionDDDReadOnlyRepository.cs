namespace Tech.Challenge.I.Domain.Repositories.DDD;
public interface IRegionDDDReadOnlyRepository
{
    Task<bool> ThereIsDDDNumber(int ddd);
    Task<IEnumerable<Entities.RegionDDD>> RecoverAllAsync();
    Task<IEnumerable<Entities.RegionDDD>> RecoverListDDDByRegionAsync(string region);
    Task<IEnumerable<Entities.RegionDDD>> RecoverListByDDDAsync(int dDD);
    Task<Entities.RegionDDD> RecoverByDDDAsync(int dDD);
    Task<Entities.RegionDDD> RecoverByIdAsync(Guid id);
}
