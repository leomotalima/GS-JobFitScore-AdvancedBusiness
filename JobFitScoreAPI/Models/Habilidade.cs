using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("habilidades")]
    public class Habilidade
    {
        [Key]
        [Column("id_habilidade")]
        public int IdHabilidade { get; set; }

        [Required]
        [Column("nome")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;
    }
}
