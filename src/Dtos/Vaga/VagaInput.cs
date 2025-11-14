using System.ComponentModel.DataAnnotations;

namespace JobFitScoreAPI.Dtos.Vaga
{
    public class VagaInput
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nível de experiência é obrigatório.")]
        public string NivelExperiencia { get; set; } = string.Empty;

        [Required(ErrorMessage = "O salário é obrigatório.")]
        public decimal? Salario { get; set; }

        public string? Localizacao { get; set; }
    }
}
