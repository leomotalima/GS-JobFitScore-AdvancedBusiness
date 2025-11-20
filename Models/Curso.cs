using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("CURSOS")]
    public class Curso
    {
        [Key]
        [Column("ID_CURSO")]
        public int IdCurso { get; set; }

        [Required]
        [Column("NOME")]
        [MaxLength(150)]
        public string Nome { get; set; } = string.Empty;

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

        
        public Usuario? Usuario { get; set; }
    }
}
