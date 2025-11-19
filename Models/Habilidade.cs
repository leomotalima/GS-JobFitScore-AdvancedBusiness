using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("HABILIDADES")]
    public class Habilidade
    {
        [Key]
        [Column("ID_HABILIDADE")]
        public int IdHabilidade { get; set; }

        [Required]
        [Column("NOME")]
        [MaxLength(100)]
        public string NomeHabilidade { get; set; } = string.Empty;

        [Column("CATEGORIA")]
        [MaxLength(100)]
        public string? Categoria { get; set; }

        [Column("DESCRICAO")]
        [MaxLength(500)]
        public string? Descricao { get; set; }

        // Alias legado
        [NotMapped]
        public string Nome { get => NomeHabilidade; set => NomeHabilidade = value; }
    }
}
