using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("candidaturas")]
    public class Candidatura
    {
        [Key]
        [Column("id_candidatura")]
        public int IdCandidatura { get; set; }

        [Required]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Required]
        [Column("vaga_id")]
        public int VagaId { get; set; }

        [Column("data_candidatura")]
        public DateTime DataCandidatura { get; set; } = DateTime.Now; // Alinha com DEFAULT sysdate

        [Column("status")]
        [MaxLength(50)]
        public string Status { get; set; } = "Em Análise"; // Valor padrão alinhado com o CHECK

        // Navegação
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [ForeignKey("VagaId")]
        public Vaga? Vaga { get; set; }
    }
}
