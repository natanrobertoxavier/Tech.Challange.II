using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.User;
using Tech.Challenge.I.Integration.Tests.Fakes;

namespace Tech.Challenge.I.Integration.Tests.WebAPI.Controllers.Base;
public abstract class  BaseControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _httpClient;
    protected static string ApplicationUrl => "https://localhost:7020/";
    protected string ApiUrl => Path.Combine(ApplicationUrl);

    public BaseControllerTest()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegratedTests");

        var _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddJsonFile("appsettings.IntegratedTests.json", optional: false, reloadOnChange: true);
                });

                builder.ConfigureServices(services =>
                {
                    ReplaceRepositories(services);
                });

                builder.UseEnvironment("IntegratedTests");
                builder.UseUrls(ApplicationUrl);
            });

        _httpClient = _factory.CreateClient();
    }

    private static void ReplaceRepositories(IServiceCollection services)
    {
        ReplaceService<IUserReadOnlyRepository, FakeUserReadOnlyReposytory>(services);
        ReplaceService<IUserWriteOnlyRepository, FakeUserReadOnlyReposytory>(services);
        ReplaceService<IUserUpdateOnlyRepository, FakeUserReadOnlyReposytory>(services);
        ReplaceService<IWorkUnit, FakeWorkUnit>(services);
    }

    private static void ReplaceService<TInterface, TImplementation>(IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {    
            var serviceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TInterface));

        if (serviceDescriptor != null)
            services.Remove(serviceDescriptor);

        services.AddTransient<TInterface, TImplementation>();
    }
}
