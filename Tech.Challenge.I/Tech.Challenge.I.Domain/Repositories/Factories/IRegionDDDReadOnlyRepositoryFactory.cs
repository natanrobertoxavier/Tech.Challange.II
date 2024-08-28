using Microsoft.Extensions.DependencyInjection;
using Tech.Challenge.I.Domain.Repositories.DDD;

namespace Tech.Challenge.I.Domain.Repositories.Factories;
public interface IRegionDDDReadOnlyRepositoryFactory
{
    (IRegionDDDReadOnlyRepository repository, IServiceScope scope) Create();
}
