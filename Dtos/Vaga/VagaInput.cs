namespace JobFitScoreAPI.Dtos.Vaga
{
    public class VagaInput
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? NivelExperiencia { get; set; }
        public decimal? Salario { get; set; }
        public string? Localizacao { get; set; }

        public int IdEmpresa { get; set; }
    }
}
