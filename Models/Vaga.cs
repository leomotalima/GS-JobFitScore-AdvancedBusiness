using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JobFitScoreAPI.Models
{
    [Table("VAGAS")]
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

        [JsonIgnore] 
        [ForeignKey(nameof(EmpresaId))]
        public Empresa? Empresa { get; set; }
    }
}
