using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("USUARIOS")]
    public class Usuario
    {
        [Key]
        [Column("ID_USUARIO")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("NOME")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Column("EMAIL")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("SENHA")]
        [MaxLength(200)]
        public string Senha { get; set; } = string.Empty;

        [Column("REFRESH_TOKEN")]
        [MaxLength(200)]
        public string? RefreshToken { get; set; }

        [Column("EXPIRA_REFRESH_TOKEN")]
        public DateTime? ExpiraRefreshToken { get; set; }
    }
}
