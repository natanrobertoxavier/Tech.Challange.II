using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Tech.Challenge.I.Domain.Entities;

namespace Tech.Challenge.I.Integration.Tests.Api;
public abstract class BaseTestClient
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory<Program> Factory;
    protected readonly string ControllerUri;

    protected BaseTestClient(
        string controllerUri)
    {
        ControllerUri = controllerUri;
        Factory = new CustomWebApplicationFactory<Program>();
        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions());
    }

    protected static async Task<T> DeserializeResponse<T>(HttpResponseMessage response) =>
        JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());

}
