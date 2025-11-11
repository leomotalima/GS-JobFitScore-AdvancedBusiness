using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tabelas do banco de dados Oracle
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Vaga> Vagas { get; set; } = null!;
        public DbSet<Candidatura> Candidaturas { get; set; } = null!;
        public DbSet<Curso> Cursos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeamento das tabelas
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Vaga>().ToTable("Vagas");
            modelBuilder.Entity<Candidatura>().ToTable("Candidaturas");
            modelBuilder.Entity<Curso>().ToTable("Cursos");

            // Relacionamentos
            modelBuilder.Entity<Candidatura>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Candidaturas)
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Candidatura>()
                .HasOne(c => c.Vaga)
                .WithMany(v => v.Candidaturas)
                .HasForeignKey(c => c.IdVaga)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
