using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

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

        [HttpGet(Name = "GetCursos")]
        [SwaggerOperation(Summary = "Lista todos os cursos", Description = "Retorna uma lista paginada de cursos.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Lista de cursos retornada com sucesso")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno no servidor")]
        public async Task<IActionResult> GetCursos(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);

            var totalItems = await _context.Cursos.CountAsync();

            var cursos = await _context.Cursos
                .OrderBy(c => c.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CursoOutput
                {
                    IdCurso = c.IdCurso,
                    Nome = c.Nome ?? string.Empty, // Corrigido: tratamento para nulo
                    Instituicao = c.Instituicao ?? string.Empty, // Corrigido: tratamento para nulo
                    CargaHoraria = (int?)c.CargaHoraria ?? 0 // Corrigido: tratamento para nulo
                })
                .ToListAsync();

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(ApiResponse<object>.Ok(new { meta, data = cursos }, "Cursos listados com sucesso."));
        }

        [HttpGet("{id}", Name = "GetCurso")]
        [SwaggerOperation(Summary = "Obtém um curso específico", Description = "Retorna os detalhes de um curso pelo ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Curso encontrado com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Curso não encontrado")]
        public async Task<IActionResult> GetCurso(int id)
        {
            var curso = await _context.Cursos
                .Where(c => c.IdCurso == id)
                .Select(c => new CursoOutput
                {
                    IdCurso = c.IdCurso,
                    Nome = c.Nome ?? string.Empty, // Corrigido: tratamento para nulo
                    Instituicao = c.Instituicao ?? string.Empty, // Corrigido: tratamento para nulo
                    CargaHoraria = (int?)c.CargaHoraria ?? 0 // Corrigido: tratamento para nulo
                })
                .FirstOrDefaultAsync();

            if (curso == null)
                return NotFound(ApiResponse<string>.Fail("Curso não encontrado."));

            return Ok(ApiResponse<CursoOutput>.Ok(curso, "Curso encontrado com sucesso."));
        }

        [HttpPost(Name = "CreateCurso")]
        [SwaggerOperation(Summary = "Cria um novo curso", Description = "Adiciona um novo curso no sistema.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Curso criado com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro na requisição ou dados inválidos")]
        public async Task<IActionResult> CreateCurso([FromBody] CursoInput input)
        {
            if (input == null)
                return BadRequest(ApiResponse<string>.Fail("Input não pode ser nulo."));

            var curso = new Curso
            {
                Nome = input.Nome ?? string.Empty, // Corrigido: tratamento para nulo
                Instituicao = input.Instituicao ?? string.Empty, // Corrigido: tratamento para nulo
                CargaHoraria = input.CargaHoraria
            };

            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var output = new CursoOutput
            {
                IdCurso = curso.IdCurso,
                Nome = curso.Nome ?? string.Empty, // Corrigido: tratamento para nulo
                Instituicao = curso.Instituicao ?? string.Empty, // Corrigido: tratamento para nulo
                CargaHoraria = (int?)curso.CargaHoraria ?? 0 // Corrigido: tratamento para nulo
            };

            return CreatedAtAction(nameof(GetCurso), new { id = curso.IdCurso },
                ApiResponse<CursoOutput>.Ok(output, "Curso criado com sucesso."));
        }

        [HttpPut("{id}", Name = "UpdateCurso")]
        [SwaggerOperation(Summary = "Atualiza um curso existente", Description = "Modifica informações de um curso.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Curso atualizado com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro de validação ou dados inválidos")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Curso não encontrado")]
        public async Task<IActionResult> UpdateCurso(int id, [FromBody] CursoInput input)
        {
            if (input == null)
                return BadRequest(ApiResponse<string>.Fail("Input não pode ser nulo."));

            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
                return NotFound(ApiResponse<string>.Fail("Curso não encontrado."));

            // Corrigido: tratamento adequado para valores nulos
            curso.Nome = !string.IsNullOrEmpty(input.Nome) ? input.Nome : curso.Nome;
            curso.Instituicao = !string.IsNullOrEmpty(input.Instituicao) ? input.Instituicao : curso.Instituicao;
            curso.CargaHoraria = input.CargaHoraria != 0 ? input.CargaHoraria : curso.CargaHoraria;

            _context.Entry(curso).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var output = new CursoOutput
            {
                IdCurso = curso.IdCurso,
                Nome = curso.Nome ?? string.Empty, // Corrigido: tratamento para nulo
                Instituicao = curso.Instituicao ?? string.Empty, // Corrigido: tratamento para nulo
                CargaHoraria = (int?)curso.CargaHoraria ?? 0 // Corrigido: tratamento para nulo
            };

            return Ok(ApiResponse<CursoOutput>.Ok(output, "Curso atualizado com sucesso."));
        }

        [HttpDelete("{id}", Name = "DeleteCurso")]
        [SwaggerOperation(Summary = "Remove um curso", Description = "Exclui um curso cadastrado do sistema.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Curso removido com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Curso não encontrado")]
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