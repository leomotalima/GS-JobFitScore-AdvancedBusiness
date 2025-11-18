using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("EMPRESAS")]
    public class Empresa
    {
        [Key]
        [Column("id_empresa")]
        public int IdEmpresa { get; set; }

        [Required]
        [Column("nome")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Column("cnpj")]
        [MaxLength(14)]
        public string Cnpj { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("senha")]
        [MaxLength(200)]
        public string Senha { get; set; } = string.Empty;

        [Column("refresh_token")]
        [MaxLength(200)]
        public string? RefreshToken { get; set; }

        [Column("expira_refresh_token")]
        public DateTime? ExpiraRefreshToken { get; set; }

        // Relacionamento 1:N com vagas
        public ICollection<Vaga>? Vagas { get; set; }
    }
}
