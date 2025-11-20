using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("CANDIDATURAS")]
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
        public DateTime DataCandidatura { get; set; } = DateTime.Now; 

        [Column("status")]
        [MaxLength(50)]
        public string Status { get; set; } = "Em Análise"; 

        
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }

        [ForeignKey(nameof(VagaId))]
        public Vaga? Vaga { get; set; }
    }
}
