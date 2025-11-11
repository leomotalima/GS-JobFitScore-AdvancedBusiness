using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("senha")]
        public string Senha { get; set; } = string.Empty;

        [Column("refresh_token")]
        public string? RefreshToken { get; set; }

        [Column("expira_refresh_token")]
        public DateTime? ExpiraRefreshToken { get; set; }

        [Column("habilidades")]
        public string? Habilidades { get; set; }

        public ICollection<Candidatura>? Candidaturas { get; set; }
    }
}
