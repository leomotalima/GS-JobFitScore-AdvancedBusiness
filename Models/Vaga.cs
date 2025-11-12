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

        [Column("descricao")]
        public string? Descricao { get; set; }

        [Column("nivel_experiencia")]
        public string? NivelExperiencia { get; set; }

        [Column("salario")]
        public decimal? Salario { get; set; }

        [Column("localizacao")]
        public string? Localizacao { get; set; }

        [ForeignKey("Empresa")]
        [Column("id_empresa")]
        public int? IdEmpresa { get; set; }

        public Empresa? Empresa { get; set; }
        public ICollection<Candidatura>? Candidaturas { get; set; }
    }
}
