using System.ComponentModel.DataAnnotations;

namespace JobFitScoreAPI.Dtos.Usuario
{
    public class UsuarioUpdateInput
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Email { get; set; } = string.Empty;

        public string? Habilidades { get; set; }
    }
}
