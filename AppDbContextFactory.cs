using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;

namespace JobFitScoreAPI.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            Env.Load();

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__PostgresConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string não encontrada no .env");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
