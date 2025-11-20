using System;

namespace JobFitScoreAPI.Dtos.Curso
{
    public class CursoInput
    {
        public string Nome { get; set; } = string.Empty;
        public string? Instituicao { get; set; }
        public int? CargaHoraria { get; set; }
        public int UsuarioId { get; set; }

        // Adicione estes campos
        public DateTime? DataConclusao { get; set; }
        public string? Descricao { get; set; }
    }
}
