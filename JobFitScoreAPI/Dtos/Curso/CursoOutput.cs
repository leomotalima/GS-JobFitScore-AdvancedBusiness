namespace JobFitScoreAPI.Dtos.Curso
{
    public class CursoOutput
    {
        public int IdCurso { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Instituicao { get; set; }
        public int? CargaHoraria { get; set; }
    }
}
