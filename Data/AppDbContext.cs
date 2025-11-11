using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ===========================================================
        // 🔹 Conjuntos de Tabelas
        // ===========================================================
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

            // ===========================================================
            // 🔹 Mapeamento das Tabelas Oracle (em MAIÚSCULO)
            // ===========================================================
            modelBuilder.Entity<Usuario>().ToTable("USUARIOS");
            modelBuilder.Entity<Empresa>().ToTable("EMPRESAS");
            modelBuilder.Entity<Vaga>().ToTable("VAGAS");
            modelBuilder.Entity<Habilidade>().ToTable("HABILIDADES");
            modelBuilder.Entity<UsuarioHabilidade>().ToTable("USUARIO_HABILIDADE");
            modelBuilder.Entity<Curso>().ToTable("CURSOS");
            modelBuilder.Entity<Candidatura>().ToTable("CANDIDATURAS");
            modelBuilder.Entity<VagaHabilidade>().ToTable("VAGA_HABILIDADE");
            modelBuilder.Entity<AuditoriaLog>().ToTable("AUDITORIA_LOG");

            // ===========================================================
            // 🔹 RELACIONAMENTOS
            // ===========================================================

            // Candidatura → Usuário
            modelBuilder.Entity<Candidatura>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Candidaturas)
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // Candidatura → Vaga
            modelBuilder.Entity<Candidatura>()
                .HasOne(c => c.Vaga)
                .WithMany(v => v.Candidaturas)
                .HasForeignKey(c => c.IdVaga)
                .OnDelete(DeleteBehavior.Cascade);

            // Vaga → Empresa
            modelBuilder.Entity<Vaga>()
                .HasOne(v => v.Empresa)
                .WithMany(e => e.Vagas)
                .HasForeignKey(v => v.IdEmpresa)
                .OnDelete(DeleteBehavior.Cascade);

            // ===========================================================
            // 🔹 Índices Únicos das Tabelas N:N
            // ===========================================================
            modelBuilder.Entity<UsuarioHabilidade>()
                .HasIndex(uh => new { uh.UsuarioId, uh.HabilidadeId })
                .IsUnique();

            modelBuilder.Entity<VagaHabilidade>()
                .HasIndex(vh => new { vh.VagaId, vh.HabilidadeId })
                .IsUnique();

            // ===========================================================
            // 🔹 Ajusta nomes de tabela (Oracle usa MAIÚSCULO)
            // ===========================================================
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName))
                    entity.SetTableName(tableName.ToUpper());
            }
        }
    }
}
