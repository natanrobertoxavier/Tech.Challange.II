using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Tech.Challenge.I.Domain.Extension;
using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Domain.Repositories.Contact;
using Tech.Challenge.I.Domain.Repositories.DDD;
using Tech.Challenge.I.Domain.Repositories.Factories;
using Tech.Challenge.I.Domain.Repositories.User;
using Tech.Challenge.I.Infrastructure.RepositoryAccess;
using Tech.Challenge.I.Infrastructure.RepositoryAccess.Factories;
using Tech.Challenge.I.Infrastructure.RepositoryAccess.Repository;

namespace Tech.Challenge.I.Infrastructure;
public static class Initializer
{
    public static void AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configurationManager, 
        bool isIntegrationTest)
    {
        AddFluentMigrator(services, configurationManager, isIntegrationTest);
        AddContext(services, configurationManager, isIntegrationTest);

        if (!isIntegrationTest)
        {
            AddRepositories(services);
            AddWorkUnit(services);
            AddFactories(services);
        }
    }

    private static void AddWorkUnit(IServiceCollection services)
    {
        services.AddScoped<IWorkUnit, WorkUnit>();
    }

    private static void AddFluentMigrator(
        IServiceCollection services, 
        IConfiguration configurationManager,
        bool isIntegrationTest)
    {
        if (!isIntegrationTest)
        {
            services.AddFluentMigratorCore().ConfigureRunner(c =>
                 c.AddMySql5()
                  .WithGlobalConnectionString(configurationManager.GetFullConnection())
                  .ScanIn(Assembly.Load("Tech.Challenge.I.Infrastructure")).For.All());
        }
        else
        {
            services.AddFluentMigratorCore().ConfigureRunner(c =>
            c.AddMySql5()
                  .WithGlobalConnectionString(configurationManager.GetFullConnectionIntegrationTest())
                  .ScanIn(Assembly.Load("Tech.Challenge.I.Infrastructure")).For.All());
        }
    }

    private static void AddContext(
        IServiceCollection services, 
        IConfiguration configurationManager,
        bool isIntegrationTest)
    {
        if (!isIntegrationTest)
        {
            var versaoServidor = new MySqlServerVersion(new Version(8, 0, 26));
            var connectionString = configurationManager.GetFullConnection();

            services.AddDbContext<TechChallengeContext>(dbContextoOpcoes =>
            {
                dbContextoOpcoes.UseMySql(connectionString, versaoServidor);
            });
        }
        else
        {
            var versaoServidor = new MySqlServerVersion(new Version(8, 0, 26));
            var connectionString = configurationManager.GetFullConnectionIntegrationTest();

            services.AddDbContext<TechChallengeContext>(dbContextoOpcoes =>
            {
                dbContextoOpcoes.UseMySql(connectionString, versaoServidor);
            });
        }
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services
            .AddScoped<IUserReadOnlyRepository, UserRepository>()
            .AddScoped<IUserWriteOnlyRepository, UserRepository>()
            .AddScoped<IUserUpdateOnlyRepository, UserRepository>()
            .AddScoped<IRegionDDDReadOnlyRepository, RegionDDDRepository>()
            .AddScoped<IRegionDDDWriteOnlyRepository, RegionDDDRepository>()
            .AddScoped<IContactReadOnlyRepository, ContactRepository>()
            .AddScoped<IContactWriteOnlyRepository, ContactRepository>();
    }

    private static void AddFactories(IServiceCollection services)
    {
        services
            .AddSingleton<IRegionDDDReadOnlyRepositoryFactory, RegionDDDReadOnlyRepositoryFactory>();
    }
}
