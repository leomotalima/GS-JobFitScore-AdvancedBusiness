using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobFitScoreAPI.Models
{
    [Table("auditoria_log")]
    public class AuditoriaLog
    {
        [Key]
        [Column("id_auditoria")]
        public int IdAuditoria { get; set; }

        [Required]
        [Column("nome_tabela")]
        public string NomeTabela { get; set; } = string.Empty;

        [Required]
        [Column("operacao")]
        public string Operacao { get; set; } = string.Empty;

        [Column("registro_id")]
        public int? RegistroId { get; set; }

        [Column("usuario_banco")]
        public string? UsuarioBanco { get; set; }

        [Required]
        [Column("data_operacao")]
        public DateTime DataOperacao { get; set; }

        [Column("detalhe")]
        public string? Detalhe { get; set; }
    }
}
