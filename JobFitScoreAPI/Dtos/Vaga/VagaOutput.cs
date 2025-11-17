namespace JobFitScoreAPI.Dtos.Vaga
{
    public class VagaOutput
    {
        public int IdVaga { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string NivelExperiencia { get; set; } = string.Empty;
        public decimal? Salario { get; set; }
        public string? Localizacao { get; set; }
    }
}
