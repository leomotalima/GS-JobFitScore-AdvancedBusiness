using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Empresa> Empresas { get; set; } = null!;
        public DbSet<Vaga> Vagas { get; set; } = null!;
        public DbSet<Habilidade> Habilidades { get; set; } = null!;
        public DbSet<UsuarioHabilidade> UsuarioHabilidades { get; set; } = null!;
        public DbSet<Curso> Cursos { get; set; } = null!;
        public DbSet<Candidatura> Candidaturas { get; set; } = null!;
        public DbSet<VagaHabilidade> VagaHabilidades { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("USUARIOS");
            modelBuilder.Entity<Empresa>().ToTable("EMPRESAS");
            modelBuilder.Entity<Vaga>().ToTable("VAGAS");
            modelBuilder.Entity<Habilidade>().ToTable("HABILIDADES");
            modelBuilder.Entity<UsuarioHabilidade>().ToTable("USUARIO_HABILIDADE");
            modelBuilder.Entity<Curso>().ToTable("CURSOS");
            modelBuilder.Entity<Candidatura>().ToTable("CANDIDATURAS");
            modelBuilder.Entity<VagaHabilidade>().ToTable("VAGA_HABILIDADE");

        }
    }
}