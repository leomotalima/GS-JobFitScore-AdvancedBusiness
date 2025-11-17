namespace JobFitScoreAPI.Dtos.Usuario
{
    public class UsuarioOutput
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Habilidades { get; set; }
    }
}
