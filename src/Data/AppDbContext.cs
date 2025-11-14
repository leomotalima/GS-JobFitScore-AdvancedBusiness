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
        public DbSet<AuditoriaLog> AuditoriaLogs { get; set; } = null!;

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
            modelBuilder.Entity<AuditoriaLog>().ToTable("AUDITORIA_LOG");

            // Entidade sem chave primária
            modelBuilder.Entity<AuditoriaLog>().HasNoKey();

            // Chave composta das tabelas N:N
            modelBuilder.Entity<UsuarioHabilidade>()
                .HasKey(uh => new { uh.UsuarioId, uh.HabilidadeId });

            modelBuilder.Entity<VagaHabilidade>()
                .HasKey(vh => new { vh.VagaId, vh.HabilidadeId });

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

            modelBuilder.Entity<Vaga>()
                .HasOne(v => v.Empresa)
                .WithMany(e => e.Vagas)
                .HasForeignKey(v => v.IdEmpresa)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices únicos
            modelBuilder.Entity<UsuarioHabilidade>()
                .HasIndex(uh => new { uh.UsuarioId, uh.HabilidadeId })
                .IsUnique();

            modelBuilder.Entity<VagaHabilidade>()
                .HasIndex(vh => new { vh.VagaId, vh.HabilidadeId })
                .IsUnique();

            // Converte nomes das tabelas para maiúsculo
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName))
                {
                    entity.SetTableName(tableName.ToUpper());
                }
            }
        }
    }
}
