using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Dtos.Habilidade;
using Swashbuckle.AspNetCore.Annotations;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/habilidades")]
    [Tags("Habilidades")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize]
    public class HabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public HabilidadeController(AppDbContext context, LinkGenerator linkGenerator)
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

        // GET - Listar habilidades
        [HttpGet(Name = "GetHabilidades")]
        [SwaggerOperation(Summary = "Lista todas as habilidades", Description = "Retorna uma lista paginada de habilidades.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Lista retornada com sucesso")]
        public async Task<IActionResult> GetHabilidades(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);

            var totalItems = await _context.Habilidades.CountAsync();

            var habilidades = await _context.Habilidades
                .OrderBy(h => h.NomeHabilidade) // <<--- CORREÇÃO: Usando a propriedade mapeada
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(h => new HabilidadeOutput
                {
                    IdHabilidade = h.IdHabilidade,
                    Nome = h.NomeHabilidade // <<--- CORREÇÃO: Usando a propriedade mapeada para o DTO
                })
                .ToListAsync();

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(ApiResponse<object>.Ok(new { meta, data = habilidades }, "Habilidades listadas com sucesso."));
        }

        // GET - Buscar habilidade por ID
        [HttpGet("{id}", Name = "GetHabilidade")]
        [SwaggerOperation(Summary = "Obtém uma habilidade específica", Description = "Retorna os detalhes de uma habilidade pelo ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Habilidade encontrada com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Habilidade não encontrada")]
        public async Task<IActionResult> GetHabilidade(int id)
        {
            var habilidade = await _context.Habilidades
                .Where(h => h.IdHabilidade == id)
                .Select(h => new HabilidadeOutput
                {
                    IdHabilidade = h.IdHabilidade,
                    Nome = h.NomeHabilidade // <<--- CORREÇÃO: Usando a propriedade mapeada para o DTO
                })
                .FirstOrDefaultAsync();

            if (habilidade == null)
                return NotFound(ApiResponse<string>.Fail("Habilidade não encontrada."));

            var result = new
            {
                habilidade,
                links = new List<object>
                {
                    new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
                }
            };

            return Ok(ApiResponse<object>.Ok(result, "Habilidade encontrada com sucesso."));
        }

        // POST - Criar habilidade
        [HttpPost(Name = "CreateHabilidade")]
        [SwaggerOperation(Summary = "Cria uma nova habilidade", Description = "Adiciona uma nova habilidade no sistema.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Habilidade criada com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro na requisição")]
        public async Task<IActionResult> CreateHabilidade([FromBody] HabilidadeInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Nome))
                return BadRequest(ApiResponse<string>.Fail("Dados inválidos."));

            var habilidade = new Habilidade
            {
                NomeHabilidade = input.Nome // <<--- CORREÇÃO: Definindo a propriedade mapeada
            };

            _context.Habilidades.Add(habilidade);
            await _context.SaveChangesAsync();

            var output = new HabilidadeOutput
            {
                IdHabilidade = habilidade.IdHabilidade,
                Nome = habilidade.NomeHabilidade // <<--- CORREÇÃO: Usando a propriedade mapeada para o DTO
            };

            return CreatedAtAction(nameof(GetHabilidade), new { id = habilidade.IdHabilidade },
                ApiResponse<HabilidadeOutput>.Ok(output, "Habilidade criada com sucesso."));
        }

        // PUT - Atualizar habilidade
        [HttpPut("{id}", Name = "UpdateHabilidade")]
        [SwaggerOperation(Summary = "Atualiza uma habilidade existente")]
        [SwaggerResponse(StatusCodes.Status200OK, "Habilidade atualizada com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Habilidade não encontrada")]
        public async Task<IActionResult> UpdateHabilidade(int id, [FromBody] HabilidadeInput input)
        {
            if (input == null)
                return BadRequest(ApiResponse<string>.Fail("Input não pode ser nulo."));

            var habilidade = await _context.Habilidades.FindAsync(id);
            if (habilidade == null)
                return NotFound(ApiResponse<string>.Fail("Habilidade não encontrada."));

            habilidade.NomeHabilidade = input.Nome ?? habilidade.NomeHabilidade; // <<--- CORREÇÃO: Usando a propriedade mapeada

            await _context.SaveChangesAsync();

            var output = new HabilidadeOutput
            {
                IdHabilidade = habilidade.IdHabilidade,
                Nome = habilidade.NomeHabilidade // <<--- CORREÇÃO: Usando a propriedade mapeada para o DTO
            };

            return Ok(ApiResponse<HabilidadeOutput>.Ok(output, "Habilidade atualizada com sucesso."));
        }

        // DELETE - Remover habilidade
        [HttpDelete("{id}", Name = "DeleteHabilidade")]
        [SwaggerOperation(Summary = "Remove uma habilidade", Description = "Exclui uma habilidade cadastrada no sistema.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Habilidade removida com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Habilidade não encontrada")]
        public async Task<IActionResult> DeleteHabilidade(int id)
        {
            var habilidade = await _context.Habilidades.FindAsync(id);
            if (habilidade == null)
                return NotFound(ApiResponse<string>.Fail("Habilidade não encontrada."));

            _context.Habilidades.Remove(habilidade);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // MÉTODOS AUXILIARES HATEOAS
        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetHabilidade), "Habilidade", new { id }) ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetHabilidades), "Habilidade", new { page, pageSize }) ?? string.Empty;
    }
}