using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("vagas")]
    public class Vaga
    {
        [Key]
        [Column("id_vaga")]
        public int IdVaga { get; set; }

        [Required]
        [Column("titulo")]
        [MaxLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Column("descricao")]
        [MaxLength(500)]
        public string? Descricao { get; set; }

        [Column("requisitos")]
        [MaxLength(500)]
        public string? Requisitos { get; set; }

        [Column("localizacao")]
        [MaxLength(150)]
        public string? Localizacao { get; set; }

        [Column("salario")]
        public decimal? Salario { get; set; }

        [Column("data_publicacao")]
        public DateTime? DataPublicacao { get; set; }

        [Required]
        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        // Navegação para empresa
        [ForeignKey("EmpresaId")]
        public Empresa? Empresa { get; set; }
    }
}
