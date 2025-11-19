using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("EMPRESAS")]
    public class Empresa
    {
        [Key]
        [Column("ID_EMPRESA")]
        public int IdEmpresa { get; set; }

        // Nome fantasia / razão social
        [Required]
        [Column("NOME")]
        [MaxLength(100)]
        public string NomeEmpresa { get; set; } = string.Empty;

        [Required]
        [Column("CNPJ")]
        [MaxLength(14)]
        public string Cnpj { get; set; } = string.Empty;

        [Required]
        [Column("EMAIL")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("SENHA")]
        [MaxLength(200)]
        public string Senha { get; set; } = string.Empty;

        [Column("TELEFONE")]
        [MaxLength(20)]
        public string? Telefone { get; set; }

        [Column("SETOR")]
        [MaxLength(100)]
        public string? Setor { get; set; }

        [Column("DESCRICAO")]
        [MaxLength(500)]
        public string? Descricao { get; set; }

        [Column("REFRESH_TOKEN")]
        [MaxLength(200)]
        public string? RefreshToken { get; set; }

        [Column("EXPIRA_REFRESH_TOKEN")]
        public DateTime? ExpiraRefreshToken { get; set; }

        // Relacionamento 1:N com vagas
        public ICollection<Vaga>? Vagas { get; set; }

        // Alias legado para compatibilidade com controllers que usam Nome
        [NotMapped]
        public string Nome { get => NomeEmpresa; set => NomeEmpresa = value; }
    }
}
