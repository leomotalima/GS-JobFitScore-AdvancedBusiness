using JobFitScore.Tests;
using System.Net;
using Xunit;


public class HealthCheckTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthCheckTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_Retornar_HealthCheck_Padrao()
    {
        var response = await _client.GetAsync("/api/health/ping");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();

        Assert.Contains("\"success\":true", json);
        Assert.Contains("\"API rodando com sucesso\"", json);
        Assert.Contains("\"status\":\"Healthy\"", json);

        Assert.Contains("\"environment\"", json);
        Assert.Contains("\"timestampUtc\"", json);
    }
}
