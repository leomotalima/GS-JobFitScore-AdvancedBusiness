using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScore.Tests.Integration;

namespace JobFitScore.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove DbContext original
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Adiciona InMemory DB
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("JobFitScore_TestDB"));

                // Autenticação fake
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                // Inicializa os dados de teste no banco InMemory
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Garante que o banco seja limpo antes de rodar os testes
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // Insere um usuário de teste no banco
                db.Usuarios.Add(new Usuario
                {
                    Nome = "Login Teste",
                    Email = "login@teste.com",
                    Senha = BCrypt.Net.BCrypt.HashPassword("123456")
                });

                db.SaveChanges();
            });
        }
    }
}
