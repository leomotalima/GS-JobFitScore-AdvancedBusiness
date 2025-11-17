using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace JobFitScore.Tests
{
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
            var loginData = new
            {
                email = "login@teste.com",
                senha = "123456"
            };

            var response = await _client.PostAsJsonAsync("/api/v1/login", loginData);

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json); // Debug

            response.EnsureSuccessStatusCode(); // 200 OK

            Assert.Contains("token", json.ToLower());
            Assert.Contains("login@teste.com", json.ToLower());
        }
    }
}
