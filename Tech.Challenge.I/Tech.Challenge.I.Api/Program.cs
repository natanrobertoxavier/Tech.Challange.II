using Tech.Challenge.I.Api.Filters;
using Tech.Challenge.I.Application;
using Tech.Challenge.I.Application.Services.Automapper;
using Tech.Challenge.I.Communication;
using Tech.Challenge.I.Domain.Extension;
using Tech.Challenge.I.Infrastructure;
using Tech.Challenge.I.Infrastructure.Migrations;
using Tech.Challenge.I.Infrastructure.RepositoryAccess;

var builder = WebApplication.CreateBuilder(args);

var currentEnvironment = builder.Environment.EnvironmentName;

builder.Services.AddRouting(option => option.LowercaseUrls = true);

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new DescriptionEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer",
        new Microsoft.OpenApi.Models
        .OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "JWT Authorization header utilizando o Bearer sheme. Exemple: \"Authorization: Bearer {token}\"",
        });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    c.SchemaFilter<EnumSchemaFilter>();
});

builder.Services.AddInfrastructure(builder.Configuration, IsIntegrationTests());
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilters)));

builder.Services.AddScoped(provider => new AutoMapper.MapperConfiguration(cfg =>
{
    cfg.AddProfile(new TechChallengeProfile());
}).CreateMapper());

builder.Services.AddScoped<AuthenticatedUserAttribute>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

UpdateDatabase();

app.MapControllers();

app.Run();

void UpdateDatabase()
{
    using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    using var context = serviceScope.ServiceProvider.GetService<TechChallengeContext>();

    if (!IsIntegrationTests())
    {
        var connection = builder.Configuration.GetConnection();
        var databaseName = builder.Configuration.GetDatabaseName();

        Database.CreateDatabase(connection, databaseName);

        app.MigrateDatabase();
    }
    else
    {
        var connection = builder.Configuration.GetConnection();
        var databaseName = builder.Configuration.GetIntegrationTestDatabaseName();

        Environment.SetEnvironmentVariable("ConnectionString", connection);
        Environment.SetEnvironmentVariable("IntegrationTestDatabase", databaseName);

        Database.CreateDatabase(connection, databaseName);

        app.MigrateDatabase();
    }
}

bool IsIntegrationTests() => currentEnvironment == "IntegratedTests";

public partial class Program { }