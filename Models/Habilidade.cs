namespace JobFitScoreAPI.Models
{
    public class Habilidade
    {
        public int IdHabilidade { get; set; }
        public string Nome { get; set; } = string.Empty;

        // Relacionamentos N:N
        public ICollection<UsuarioHabilidade>? UsuarioHabilidades { get; set; }
        public ICollection<VagaHabilidade>? VagaHabilidades { get; set; }
    }
}
