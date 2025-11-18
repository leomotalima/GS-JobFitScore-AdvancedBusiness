using System.ComponentModel.DataAnnotations;

namespace JobFitScoreAPI.Dtos.Vaga
{
    public class VagaInput
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        [MaxLength(100)]
        public string Titulo { get; set; } = string.Empty;
        
    }
}