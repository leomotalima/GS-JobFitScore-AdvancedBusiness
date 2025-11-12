namespace JobFitScoreAPI.Dtos.Vaga
{
    public class VagaOutput
    {
        public int IdVaga { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? NivelExperiencia { get; set; }
        public decimal? Salario { get; set; }

                public string NomeEmpresa { get; set; } = string.Empty;
        public string EmailEmpresa { get; set; } = string.Empty;
    }
}
