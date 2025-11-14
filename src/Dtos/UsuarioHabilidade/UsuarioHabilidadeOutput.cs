namespace JobFitScoreAPI.Dtos.UsuarioHabilidade
{
    public class UsuarioHabilidadeOutput
    {
        public int IdUsuarioHabilidade { get; set; }
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;

        public int IdHabilidade { get; set; }
        public string NomeHabilidade { get; set; } = string.Empty;
        public string? DescricaoHabilidade { get; set; }
    }
}
