namespace JobFitScoreAPI.Dtos.VagaHabilidade
{
    public class VagaHabilidadeOutput
    {
        public int IdVagaHabilidade { get; set; }

        public int IdVaga { get; set; }
        public string NomeVaga { get; set; } = string.Empty;

        public int IdHabilidade { get; set; }
        public string NomeHabilidade { get; set; } = string.Empty;
    }
}
