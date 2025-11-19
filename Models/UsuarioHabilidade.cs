using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("USUARIO_HABILIDADE")]
    public class UsuarioHabilidade
    {
        [Key]
        [Column("ID_USUARIO_HABILIDADE")]
        public int IdUsuarioHabilidade { get; set; }

        [Column("USUARIO_ID")]
        public int UsuarioId { get; set; }

        [Column("HABILIDADE_ID")]
        public int HabilidadeId { get; set; }

        // Navegação
        [ForeignKey("USUARIOID")]
        public Usuario? Usuario { get; set; }

        [ForeignKey("HABILIDADEID")]
        public Habilidade? Habilidade { get; set; }
    }
}
