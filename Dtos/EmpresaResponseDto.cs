namespace JobFitScoreAPI.Dtos
{
    public class EmpresaResponseDto
    {
        public int IdEmpresa { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
