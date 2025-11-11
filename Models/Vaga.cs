using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("Vagas")]
    public class Vaga
    {
        [Key]
        [Column("id_vaga")]
        public int IdVaga { get; set; }

        [Required]
        [Column("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [Column("requisitos")]
        public string? Requisitos { get; set; }

        // 🔹 Chave estrangeira (empresa relacionada)
        [Column("id_empresa")]
        public int IdEmpresa { get; set; }

        // 🔹 Propriedade de navegação (relacionamento N:1)
        public Empresa? Empresa { get; set; }

        // 🔹 Relacionamento com candidaturas (1:N)
        public ICollection<Candidatura>? Candidaturas { get; set; }
    }
}
