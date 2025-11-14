using JobFitScore.Tests;
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class LoginIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoginIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_Fazer_Login_Com_Sucesso()
    {
        // Primeiro cria o usuário para testar login
        var novoUsuario = new
        {
            nome = "Login Teste",
            email = "login@teste.com",
            senha = "123456",
            habilidades = ""
        };

        await _client.PostAsJsonAsync("/api/v1/usuario", novoUsuario);

        // Tenta fazer login
        var loginData = new
        {
            email = "login@teste.com",
            senha = "123456"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/login", loginData);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();

        Assert.Contains("token", json.ToLower());
        Assert.Contains("login@teste.com", json.ToLower());
    }
}
