using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tech.Challenge.I.Domain.Entities;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.User;
using Tech.Challenge.I.Infrastructure.RepositoryAccess;
using Tech.Challenge.I.Integration.Tests.Fakes.Repositories;

namespace Tech.Challenge.I.Integration.Tests;
public class CustomWebApplicationFactory<TStartup>: WebApplicationFactory<TStartup> where TStartup : class
{
    private ServiceProvider _serviceProvider;
    private User _user;
    private string _password;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services => 
        {
            ReplaceDatabase(services);
            ReplaceRepositories(services);
        });

        builder.UseEnvironment("IntegrationTests");

        builder.ConfigureAppConfiguration((_, appConfiguration) => 
        { 
            appConfiguration.AddJsonFile("appsettings.IntegrationTests.json", optional: false, reloadOnChange: true);
        });
    }

    public TechChallengeContext GetContext() => 
        _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<TechChallengeContext>();

    public User RecoverUser() => _user;

    public string RecoverPassword() => _password;

    private static void ReplaceRepositories(IServiceCollection services)
    {
        ReplaceRepository<IUserReadOnlyRepository, UserRepositoryFake>(services);
        ReplaceRepository<IUserWriteOnlyRepository, UserRepositoryFake>(services);
        ReplaceRepository<IUserUpdateOnlyRepository, UserRepositoryFake>(services);
        ReplaceRepository<IWorkUnit, WorkUnitFake>(services);
    }

    private static void ReplaceRepository<TInterface, TImplementation>(IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        var serviceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TInterface));

        if (serviceDescriptor != null)
            services.Remove(serviceDescriptor);

        services.AddTransient<TInterface, TImplementation>();
    }

    private void ReplaceDatabase(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<TechChallengeContext>));

        services.Remove(descriptor);

        var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

        services.AddDbContext<TechChallengeContext>(options =>
        {
            options.UseInMemoryDatabase("InMemoryDbForTesting");
            options.UseInternalServiceProvider(provider);
        });

        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var scopeService = scope.ServiceProvider;
        var database = scopeService.GetRequiredService<TechChallengeContext>();

        database.Database.EnsureDeleted();

        (_user, _password) = ContextSeedInMemory.Seed(database);
    }

    public class HttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; } = new DefaultHttpContext();
    }
}
