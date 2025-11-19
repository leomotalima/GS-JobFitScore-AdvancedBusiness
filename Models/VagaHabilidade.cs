using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("VAGA_HABILIDADE")]
    public class VagaHabilidade
    {
        [Key]
        [Column("ID_VAGA_HABILIDADE")]
        public int IdVagaHabilidade { get; set; }

        [Column("VAGA_ID")]
        public int VagaId { get; set; }

        [Column("HABILIDADE_ID")]
        public int HabilidadeId { get; set; }

        // Navegação
        [ForeignKey("VAGAID")]
        public Vaga? Vaga { get; set; }

        [ForeignKey("HABILIDADEID")]
        public Habilidade? Habilidade { get; set; }
    }
}
