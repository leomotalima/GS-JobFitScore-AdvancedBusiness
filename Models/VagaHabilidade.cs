using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("VAGA_HABILIDADE")]
    public class VagaHabilidade
    {
        [Key]
        [Column("id_vaga_habilidade")]
        public int IdVagaHabilidade { get; set; }

        [Column("vaga_id")]
        public int VagaId { get; set; }

        [Column("habilidade_id")]
        public int HabilidadeId { get; set; }

        public Vaga? Vaga { get; set; }

        public Habilidade? Habilidade { get; set; }
    }
}