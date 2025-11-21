namespace JobFitScoreAPI.Dtos.Candidatura
{
    public class CandidaturaOutput
    {
        public int IdCandidatura { get; set; }

        public string NomeUsuario { get; set; } = string.Empty;
        public string EmailUsuario { get; set; } = string.Empty;

        public string TituloVaga { get; set; } = string.Empty;
    }
}
