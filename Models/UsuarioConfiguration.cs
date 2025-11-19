using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Models
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("USUARIOS");

            builder.HasKey(u => u.IdUsuario);

            builder.Property(u => u.IdUsuario)
                .HasColumnName("ID_USUARIO")
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Nome)
                .HasColumnName("NOME")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasColumnName("EMAIL")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Senha)
                .HasColumnName("SENHA")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(u => u.RefreshToken)
                .HasColumnName("REFRESH_TOKEN");

            builder.Property(u => u.ExpiraRefreshToken)
                .HasColumnName("EXPIRA_REFRESH_TOKEN");
        }
    }
}
