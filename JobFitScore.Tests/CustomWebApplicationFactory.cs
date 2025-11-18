using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
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

                // API Versioning
                services.AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();
                });

                services.AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

                // Build provider e criar banco
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

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
