namespace JobFitScoreAPI.Models
{
    public class VagaHabilidade
    {
        public int IdVagaHabilidade { get; set; }

        // Chaves estrangeiras
        public int VagaId { get; set; }
        public int HabilidadeId { get; set; }

        // Navegação
        public Vaga? Vaga { get; set; }
        public Habilidade? Habilidade { get; set; }
    }
}
