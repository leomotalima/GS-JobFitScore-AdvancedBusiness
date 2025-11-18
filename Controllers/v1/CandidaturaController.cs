using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Dtos.Candidatura;
using Swashbuckle.AspNetCore.Annotations;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/candidaturas")]
    [Tags("Candidaturas")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize]
    public class CandidaturaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public CandidaturaController(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        // Classe de resposta padronizada
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

        // GET - Listar candidaturas
        [HttpGet(Name = "GetCandidaturas")]
        [SwaggerOperation(Summary = "Lista todas as candidaturas", Description = "Retorna uma lista paginada de candidaturas.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Lista de candidaturas retornada com sucesso")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno no servidor")]
        public async Task<IActionResult> GetCandidaturas(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);

            var totalItems = await _context.Candidaturas.CountAsync();

            var candidaturas = await _context.Candidaturas
                .Include(c => c.Usuario)
                .Include(c => c.Vaga)
                .OrderBy(c => c.DataCandidatura)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CandidaturaOutput
                {
                    IdCandidatura = c.IdCandidatura,
                    NomeUsuario = c.Usuario!.Nome,
                    EmailUsuario = c.Usuario!.Email ?? string.Empty,
                    TituloVaga = c.Vaga!.Titulo
                })
                .ToListAsync();

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(ApiResponse<object>.Ok(new { meta, data = candidaturas }, "Candidaturas listadas com sucesso."));
        }

        // GET - Buscar candidatura por ID
        [HttpGet("{id}", Name = "GetCandidatura")]
        [SwaggerOperation(Summary = "Obtém uma candidatura específica", Description = "Retorna os detalhes de uma candidatura pelo ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Candidatura encontrada com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Candidatura não encontrada")]
        public async Task<IActionResult> GetCandidatura(int id)
        {
            var candidatura = await _context.Candidaturas
                .Include(c => c.Usuario)
                .Include(c => c.Vaga)
                .Where(c => c.IdCandidatura == id)
                .Select(c => new CandidaturaOutput
                {
                    IdCandidatura = c.IdCandidatura,
                    NomeUsuario = c.Usuario!.Nome,
                    EmailUsuario = c.Usuario!.Email ?? string.Empty,
                    TituloVaga = c.Vaga!.Titulo
                })
                .FirstOrDefaultAsync();

            if (candidatura == null)
                return NotFound(ApiResponse<string>.Fail("Candidatura não encontrada."));

            return Ok(ApiResponse<CandidaturaOutput>.Ok(candidatura, "Candidatura encontrada com sucesso."));
        }

        // POST - Criar candidatura
        [HttpPost(Name = "CreateCandidatura")]
        [SwaggerOperation(Summary = "Cria uma nova candidatura", Description = "Adiciona uma nova candidatura no sistema.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Candidatura criada com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro na requisição ou dados inválidos")]
        public async Task<IActionResult> CreateCandidatura([FromBody] CandidaturaInput input)
        {
            if (input == null)
                return BadRequest(ApiResponse<string>.Fail("Input não pode ser nulo."));

            var candidatura = new Candidatura
            {
                UsuarioId = input.IdUsuario,
                VagaId = input.IdVaga,
                Status = "Em Análise"
            };

            _context.Candidaturas.Add(candidatura);
            await _context.SaveChangesAsync();

            var usuario = await _context.Usuarios.FindAsync(input.IdUsuario);
            var vaga = await _context.Vagas.FindAsync(input.IdVaga);

            var output = new CandidaturaOutput
            {
                IdCandidatura = candidatura.IdCandidatura,
                NomeUsuario = usuario!.Nome,
                EmailUsuario = usuario!.Email ?? string.Empty,
                TituloVaga = vaga!.Titulo
            };

            return CreatedAtAction(nameof(GetCandidatura), new { id = candidatura.IdCandidatura },
                ApiResponse<CandidaturaOutput>.Ok(output, "Candidatura criada com sucesso."));
        }

        // PUT - Atualizar candidatura
        [HttpPut("{id}", Name = "UpdateCandidatura")]
        [SwaggerOperation(Summary = "Atualiza uma candidatura existente", Description = "Modifica informações de uma candidatura.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Candidatura atualizada com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro de validação ou dados inválidos")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Candidatura não encontrada")]
        public async Task<IActionResult> UpdateCandidatura(int id, [FromBody] CandidaturaInput input)
        {
            if (input == null)
                return BadRequest(ApiResponse<string>.Fail("Input não pode ser nulo."));

            var candidatura = await _context.Candidaturas.FindAsync(id);
            if (candidatura == null)
                return NotFound(ApiResponse<string>.Fail("Candidatura não encontrada."));

            candidatura.UsuarioId = input.IdUsuario != 0 ? input.IdUsuario : candidatura.UsuarioId;
            candidatura.VagaId = input.IdVaga != 0 ? input.IdVaga : candidatura.VagaId;

            await _context.SaveChangesAsync();

            var usuario = await _context.Usuarios.FindAsync(candidatura.UsuarioId);
            var vaga = await _context.Vagas.FindAsync(candidatura.VagaId);

            var output = new CandidaturaOutput
            {
                IdCandidatura = candidatura.IdCandidatura,
                NomeUsuario = usuario!.Nome,
                EmailUsuario = usuario!.Email ?? string.Empty,
                TituloVaga = vaga!.Titulo
            };

            return Ok(ApiResponse<CandidaturaOutput>.Ok(output, "Candidatura atualizada com sucesso."));
        }

        // DELETE - Remover candidatura
        [HttpDelete("{id}", Name = "DeleteCandidatura")]
        [SwaggerOperation(Summary = "Remove uma candidatura", Description = "Exclui uma candidatura cadastrada do sistema.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Candidatura removida com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Candidatura não encontrada")]
        public async Task<IActionResult> DeleteCandidatura(int id)
        {
            var candidatura = await _context.Candidaturas.FindAsync(id);
            if (candidatura == null)
                return NotFound(ApiResponse<string>.Fail("Candidatura não encontrada."));

            _context.Candidaturas.Remove(candidatura);
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }
    }
}
