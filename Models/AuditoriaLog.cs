namespace JobFitScoreAPI.Models
{
    public class AuditoriaLog
    {
        public int IdAuditoria { get; set; }
        public string NomeTabela { get; set; } = string.Empty;
        public string Operacao { get; set; } = string.Empty;
        public int? RegistroId { get; set; }
        public string? UsuarioBanco { get; set; }
        public DateTime DataOperacao { get; set; }
        public string? Detalhe { get; set; }
    }
}
