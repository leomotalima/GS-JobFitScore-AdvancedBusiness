using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using JobFitScoreAPI;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;

namespace JobFitScore.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove DbContext real
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                // Adiciona DbContext em memória
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("JobFitScore_TestDB");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Limpa banco completamente antes de criar usuário de teste
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // Cria usuário de teste
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
