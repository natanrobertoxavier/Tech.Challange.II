using Microsoft.Extensions.DependencyInjection;
using Tech.Challenge.I.Domain.Repositories.DDD;
using Tech.Challenge.I.Domain.Repositories.Factories;

namespace Tech.Challenge.I.Infrastructure.RepositoryAccess.Factories;
public class RegionDDDReadOnlyRepositoryFactory(
    IServiceScopeFactory scopeFactory) : IRegionDDDReadOnlyRepositoryFactory
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;


    public (IRegionDDDReadOnlyRepository repository, IServiceScope scope) Create()
    {
        var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRegionDDDReadOnlyRepository>();
        return (repository, scope);
    }
}
