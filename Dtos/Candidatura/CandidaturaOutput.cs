namespace JobFitScoreAPI.Dtos.Candidatura
{
    public class CandidaturaOutput
    {
        public int IdCandidatura { get; set; }

        // Usuário
        public string NomeUsuario { get; set; } = string.Empty;
        public string EmailUsuario { get; set; } = string.Empty;

        // Vaga
        public string TituloVaga { get; set; } = string.Empty;
        public string? NivelExperiencia { get; set; }
    }
}
