using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("USUARIO_HABILIDADE")]
    public class UsuarioHabilidade
    {
        [Key]
        [Column("id_usuario_habilidade")]
        public int IdUsuarioHabilidade { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("habilidade_id")]
        public int HabilidadeId { get; set; }

        
        public Usuario? Usuario { get; set; }

        public Habilidade? Habilidade { get; set; }
    }
}
