using JobFitScore.Tests;
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class UsuarioControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsuarioControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_Criar_Usuario_Com_Sucesso()
    {
        var novoUsuario = new
        {
            nome = "Teste Automatizado",
            email = "teste@automacao.com",
            senha = "123456",
            habilidades = "C#, SQL"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/usuario", novoUsuario);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Teste Automatizado", json);
        Assert.Contains("email", json.ToLower());
    }
}
