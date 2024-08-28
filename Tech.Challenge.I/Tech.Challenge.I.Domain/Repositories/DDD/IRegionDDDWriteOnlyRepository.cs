namespace Tech.Challenge.I.Domain.Repositories.DDD;
public interface IRegionDDDWriteOnlyRepository
{
    Task Add(Entities.RegionDDD ddd);
}
