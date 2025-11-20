using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("USUARIOS")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("nome")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("senha")]
        [MaxLength(200)]
        public string Senha { get; set; } = string.Empty;

        [Column("refresh_token")]
        [MaxLength(200)]
        public string? RefreshToken { get; set; }

        [Column("expira_refresh_token")]
        public DateTime? ExpiraRefreshToken { get; set; }
    }
}
