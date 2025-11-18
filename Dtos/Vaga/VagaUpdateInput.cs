namespace JobFitScoreAPI.Dtos.Vaga
{
    public class VagaUpdateInput
    {
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public string? NivelExperiencia { get; set; }
        public decimal? Salario { get; set; }
        public string? Localizacao { get; set; }
    }
}
