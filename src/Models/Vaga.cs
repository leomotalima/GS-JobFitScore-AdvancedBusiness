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

        [Required]
        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        // Navegação para empresa
        [ForeignKey("EmpresaId")]
        public Empresa? Empresa { get; set; }
    }
}
