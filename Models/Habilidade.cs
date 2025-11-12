using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("Habilidades")]
    public class Habilidade
    {
        [Key]
        [Column("id_habilidade")]
        public int IdHabilidade { get; set; }

        [Required]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        public string? Descricao { get; set; }

        public ICollection<UsuarioHabilidade>? Usuarios { get; set; }
        public ICollection<VagaHabilidade>? Vagas { get; set; }
    }
}
