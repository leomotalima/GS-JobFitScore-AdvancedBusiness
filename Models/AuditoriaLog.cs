using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("AUDITORIA_LOG")]
    public class AuditoriaLog
    {
        [Key]
        [Column("ID_AUDITORIA")]
        public int IdAuditoria { get; set; }

        [Required]
        [Column("NOME_TABELA")]
        public string NomeTabela { get; set; } = string.Empty;

        [Required]
        [Column("OPERACAO")]
        public string Operacao { get; set; } = string.Empty;

        [Column("REGISTRO_ID")]
        public int? RegistroId { get; set; }

        [Column("USUARIO_BANCO")]
        public string? UsuarioBanco { get; set; }

        [Required]
        [Column("DATA_OPERACAO")]
        public DateTime DataOperacao { get; set; }

        [Column("DETALHE")]
        public string? Detalhe { get; set; }
    }
}
