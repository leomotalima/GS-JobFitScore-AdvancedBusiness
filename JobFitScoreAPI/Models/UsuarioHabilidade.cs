using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("usuario_habilidade")]
    public class UsuarioHabilidade
    {
        [Key]
        [Column("id_usuario_habilidade")]
        public int IdUsuarioHabilidade { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("habilidade_id")]
        public int HabilidadeId { get; set; }

        // Navegação
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [ForeignKey("HabilidadeId")]
        public Habilidade? Habilidade { get; set; }
    }
}
