using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Dtos.Curso;
using Swashbuckle.AspNetCore.Annotations;

namespace JobFitScoreAPI.Controllers.v1
{
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/curso")]
[Tags("Cursos")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class CursoController : ControllerBase
{
private readonly AppDbContext _context;


    public CursoController(AppDbContext context) => _context = context;

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T? data, string message = "") =>
            new ApiResponse<T> { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string message) =>
            new ApiResponse<T> { Success = false, Message = message };
    }

    // GET - Listar cursos (paginado)
    [HttpGet(Name = "GetCursos")]
    public async Task<IActionResult> GetCursos(int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Max(pageSize, 1);

        var totalItems = await _context.Cursos.CountAsync();

        var cursos = await _context.Cursos
            .OrderBy(c => c.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .Select(c => new CursoOutput
            {
                IdCurso = c.IdCurso,
                Nome = c.Nome,
                Instituicao = c.Instituicao,
                CargaHoraria = c.CargaHoraria,
                DataConclusao = c.DataConclusao,
                Descricao = c.Descricao
            })
            .ToListAsync();

        var meta = new
        {
            totalItems,
            page,
            pageSize,
            totalPages = Math.Ceiling((double)totalItems / pageSize)
        };

        return Ok(ApiResponse<object>.Ok(new { meta, data = cursos }, "Cursos listados com sucesso."));
    }

    // GET - Buscar curso por ID
    [HttpGet("{id}", Name = "GetCurso")]
    public async Task<IActionResult> GetCurso(int id)
    {
        var curso = await _context.Cursos
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdCurso == id);

        if (curso == null)
            return NotFound(ApiResponse<string>.Fail("Curso não encontrado."));

        var output = new CursoOutput
        {
            IdCurso = curso.IdCurso,
            Nome = curso.Nome,
            Instituicao = curso.Instituicao,
            CargaHoraria = curso.CargaHoraria,
            DataConclusao = curso.DataConclusao,
            Descricao = curso.Descricao
        };

        return Ok(ApiResponse<CursoOutput>.Ok(output, "Curso encontrado com sucesso."));
    }

    // POST - Criar curso
    [HttpPost(Name = "CreateCurso")]
    public async Task<IActionResult> CreateCurso([FromBody] CursoInput input)
    {
        if (input == null)
            return BadRequest(ApiResponse<string>.Fail("Input não pode ser nulo."));

        var curso = new Curso
        {
            Nome = input.Nome,
            Instituicao = input.Instituicao,
            CargaHoraria = input.CargaHoraria,
            DataConclusao = input.DataConclusao,
            Descricao = input.Descricao,
            UsuarioId = input.UsuarioId
        };

        _context.Cursos.Add(curso);
        await _context.SaveChangesAsync();

        var output = new CursoOutput
        {
            IdCurso = curso.IdCurso,
            Nome = curso.Nome,
            Instituicao = curso.Instituicao,
            CargaHoraria = curso.CargaHoraria,
            DataConclusao = curso.DataConclusao,
            Descricao = curso.Descricao
        };

        return CreatedAtAction(nameof(GetCurso), new { id = curso.IdCurso }, ApiResponse<CursoOutput>.Ok(output, "Curso criado com sucesso."));
    }

    // PUT - Atualizar curso
    [HttpPut("{id}", Name = "UpdateCurso")]
    public async Task<IActionResult> UpdateCurso(int id, [FromBody] CursoInput input)
    {
        if (input == null)
            return BadRequest(ApiResponse<string>.Fail("Input não pode ser nulo."));

        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null)
            return NotFound(ApiResponse<string>.Fail("Curso não encontrado."));

        curso.Nome = input.Nome ?? curso.Nome;
        curso.Instituicao = input.Instituicao ?? curso.Instituicao;
        curso.CargaHoraria = input.CargaHoraria ?? curso.CargaHoraria;
        curso.DataConclusao = input.DataConclusao ?? curso.DataConclusao;
        curso.Descricao = input.Descricao ?? curso.Descricao;
        curso.UsuarioId = input.UsuarioId != 0 ? input.UsuarioId : curso.UsuarioId;

        _context.Entry(curso).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var output = new CursoOutput
        {
            IdCurso = curso.IdCurso,
            Nome = curso.Nome,
            Instituicao = curso.Instituicao,
            CargaHoraria = curso.CargaHoraria,
            DataConclusao = curso.DataConclusao,
            Descricao = curso.Descricao
        };

        return Ok(ApiResponse<CursoOutput>.Ok(output, "Curso atualizado com sucesso."));
    }

    // DELETE - Remover curso
    [HttpDelete("{id}", Name = "DeleteCurso")]
    public async Task<IActionResult> DeleteCurso(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null)
            return NotFound(ApiResponse<string>.Fail("Curso não encontrado."));

        _context.Cursos.Remove(curso);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}


}
