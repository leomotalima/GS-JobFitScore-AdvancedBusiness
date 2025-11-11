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

        [Column("empresa")]
        public string? Empresa { get; set; }

        public ICollection<Candidatura>? Candidaturas { get; set; }
    }
}
