using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JobFitScoreAPI.Models
{
    [Table("vagas")]
    public class Vaga
    {
        [Key]
        [Column("ID_VAGA")]
        public int IdVaga { get; set; }

        [Required]
        [Column("TITULO")]
        [MaxLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Column("DESCRICAO")]
        [MaxLength(500)]
        public string? Descricao { get; set; }

        [Column("REQUISITOS")]
        [MaxLength(500)]
        public string? Requisitos { get; set; }

        [Column("LOCALIZACAO")]
        [MaxLength(150)]
        public string? Localizacao { get; set; }

        [Column("SALARIO")]
        public decimal? Salario { get; set; }

        [Column("DATA_PUBLICACAO")]
        public DateTime? DataPublicacao { get; set; }

        [Required]
        [Column("EMPRESA_ID")]
        public int EmpresaId { get; set; }

        [JsonIgnore] 
        [ForeignKey(nameof(EmpresaId))]
        public Empresa? Empresa { get; set; }
    }
}
