using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("Candidaturas")]
    public class Candidatura
    {
        [Key]
        [Column("id_candidatura")]
        public int IdCandidatura { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("id_vaga")]
        public int IdVaga { get; set; }

        [Column("score")]
        public int? Score { get; set; }

        [Column("data_candidatura")]
        public DateTime DataCandidatura { get; set; } = DateTime.Now;

        // Relacionamentos
        public Usuario? Usuario { get; set; }
        public Vaga? Vaga { get; set; }
    }
}
