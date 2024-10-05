﻿using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Tech.Challenge.I.Domain.Entities;
using System.Text.Json;
using Tech.Challenge.I.Communication.Request;
using System.Text;

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

    protected async Task<string> Login(string email, string senha)
    {
        var requisicao = new RequestLoginJson
        {
            Email = email,
            Password = senha
        };

        var resposta = await PostRequest("/api/v1/login", requisicao);

        await using var respostaBody = await resposta.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(respostaBody);

        return responseData.RootElement.GetProperty("token").GetString();
    }

    protected async Task<HttpResponseMessage> PostRequest(string metodo, object body)
    {
        var jsonString = JsonConvert.SerializeObject(body);

        return await Client.PostAsync(metodo, new StringContent(jsonString, Encoding.UTF8, "application/json"));
    }

    protected async Task<HttpResponseMessage> PostRequest(string metodo, object body, string token = "")
    {
        AuthorizeRequest(token);

        var jsonString = JsonConvert.SerializeObject(body);

        return await Client.PostAsync(metodo, new StringContent(jsonString, Encoding.UTF8, "application/json"));
    }

    protected async Task<HttpResponseMessage> PutRequest(string method, object body, string token = "")
    {
        AuthorizeRequest(token);

        var jsonString = JsonConvert.SerializeObject(body);

        return await Client.PutAsync(method, new StringContent(jsonString, Encoding.UTF8, "application/json"));
    }

    private void AuthorizeRequest(string token)
    {
        if (!string.IsNullOrEmpty(token))
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }
}
