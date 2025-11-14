using System.ComponentModel.DataAnnotations;

namespace JobFitScoreAPI.Dtos.Vaga
{
    public class VagaUpdateInput
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; } = string.Empty;

        public string? NivelExperiencia { get; set; }
        public decimal? Salario { get; set; }
        public string? Localizacao { get; set; }
    }
}
