namespace JobFitScoreAPI.Models
{
    public class UsuarioHabilidade
    {
        public int IdUsuarioHabilidade { get; set; }

        // Chaves estrangeiras
        public int UsuarioId { get; set; }
        public int HabilidadeId { get; set; }

        // Navegação
        public Usuario? Usuario { get; set; }
        public Habilidade? Habilidade { get; set; }
    }
}
