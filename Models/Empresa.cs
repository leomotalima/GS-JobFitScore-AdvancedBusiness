using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("Empresas")]
    public class Empresa
    {
        [Key]
        [Column("id_empresa")]
        public int IdEmpresa { get; set; }

        [Required]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Column("cnpj")]
        public string? Cnpj { get; set; }

        // 🔹 Relacionamento com Vaga (1:N)
        public ICollection<Vaga>? Vagas { get; set; }
    }
}
