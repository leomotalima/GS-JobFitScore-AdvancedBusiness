namespace JobFitScoreAPI.Dtos.Candidatura
{
    public class CandidaturaInput
    {
        public int IdUsuario { get; set; }
        public int IdVaga { get; set; }
        public string? Observacao { get; set; }
    }
}
