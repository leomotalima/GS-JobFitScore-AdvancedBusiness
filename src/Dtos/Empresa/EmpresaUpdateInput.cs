using System.ComponentModel.DataAnnotations;

namespace JobFitScoreAPI.Dtos.Empresa
{
    public class EmpresaUpdateInput
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
    }
}
