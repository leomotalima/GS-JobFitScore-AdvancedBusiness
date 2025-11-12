namespace JobFitScoreAPI.Dtos.Usuario
{
    public class UsuarioInput
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string? Senha { get; set; }

        public string? Habilidades { get; set; }
    }
}
