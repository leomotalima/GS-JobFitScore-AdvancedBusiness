using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("CANDIDATURAS")]
    public class Candidatura
    {
        [Key]
        [Column("ID_CANDIDATURA")]
        public int IdCandidatura { get; set; }

        [Required]
        [Column("USUARIO_ID")]
        public int UsuarioId { get; set; }

        [Required]
        [Column("VAGA_ID")]
        public int VagaId { get; set; }

        [Column("DATA_CANDIDATURA")]
        public DateTime DataCandidatura { get; set; } = DateTime.Now; 

        [Column("STATUS")]
        [MaxLength(50)]
        public string Status { get; set; } = "Em Análise"; 

        // Navegação
        [ForeignKey("USUARIOID")]
        public Usuario? Usuario { get; set; }

        [ForeignKey("VAGAID")]
        public Vaga? Vaga { get; set; }
    }
}
