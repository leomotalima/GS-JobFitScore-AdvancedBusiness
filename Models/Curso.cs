using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("Cursos")]
    public class Curso
    {
        [Key]
        [Column("id_curso")]
        public int IdCurso { get; set; }

        [Column("nome")]
        public string? Nome { get; set; }

        [Column("habilidade_relacionada")]
        public string? HabilidadeRelacionada { get; set; }
    }
}
