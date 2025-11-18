using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("cursos")]
    public class Curso
    {
        [Key]
        [Column("id_curso")]
        public int IdCurso { get; set; }

        [Required]
        [Column("nome")]
        [MaxLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Column("instituicao")]
        [MaxLength(150)]
        public string? Instituicao { get; set; }

        [Column("carga_horaria")]
        public int? CargaHoraria { get; set; }

        [Required]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        // Navegação para o usuário
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }
    }
}
