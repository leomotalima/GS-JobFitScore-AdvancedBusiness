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
        public string NomeHabilidade { get; set; } = string.Empty;

        [Column("categoria")]
        [MaxLength(100)]
        public string? Categoria { get; set; }

        [Column("descricao")]
        [MaxLength(500)]
        public string? Descricao { get; set; }

        // Alias legado
        [NotMapped]
        public string Nome { get => NomeHabilidade; set => NomeHabilidade = value; }
    }
}
