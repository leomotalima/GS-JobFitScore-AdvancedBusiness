using JobFitScoreAPI.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "MinhaChaveSeguraComMaisDe32Caracteres_12345" },
                { "Jwt:Issuer", "JobFitScore" },
                { "Jwt:Audience", "JobFitScoreUsers" }
            })
            .Build();

        _jwtService = new JwtService(config);
    }

    [Fact]
    public void Deve_Gerar_Token_JWT_Valido()
    {
        string token = _jwtService.GenerateToken(1, "teste@jobfitscore.com");

        Assert.NotNull(token);
        Assert.True(token.Length > 20);
        Assert.DoesNotContain(" ", token);
        Assert.DoesNotContain("\n", token);
    }
}
