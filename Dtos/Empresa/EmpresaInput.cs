namespace JobFitScoreAPI.Dtos.Empresa
{
    public class EmpresaInput
    {
        public string Nome { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Senha usada apenas no cadastro ou troca de senha
        public string? Senha { get; set; }
    }
}
