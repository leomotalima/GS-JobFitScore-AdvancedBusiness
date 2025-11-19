using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("cursos")]
    public class Curso
    {
        [Key]
        [Column("ID_CURSO")]
        public int IdCurso { get; set; }

        [Required]
        [Column("NOME")]
        [MaxLength(150)]
        public string NomeCurso { get; set; } = string.Empty;

        [Column("INSTITUICAO")]
        [MaxLength(150)]
        public string? Instituicao { get; set; }

        [Column("CARGA_HORARIA")]
        public int? CargaHoraria { get; set; }

        [Column("DATA_CONCLUSAO")]
        public DateTime? DataConclusao { get; set; }

        [Column("DESCRICAO")]
        [MaxLength(500)]
        public string? Descricao { get; set; }

        [Required]
        [Column("USUARIO_ID")]
        public int UsuarioId { get; set; }

        // Navegação para o usuário
        [ForeignKey("USUARIOID")]
        public Usuario? Usuario { get; set; }

        // Alias legado
        [NotMapped]
        public string Nome { get => NomeCurso; set => NomeCurso = value; }
    }
}
