using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Tags("VagaHabilidade")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class VagaHabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public VagaHabilidadeController(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        // GET: api/v1/vagahabilidade?page=1&pageSize=10
        [HttpGet]
        [SwaggerOperation(Summary = "Lista todas as relações Vaga-Habilidade")]
        [SwaggerResponse(StatusCodes.Status200OK, "Relações retornadas com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Parâmetros inválidos")]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { mensagem = "Parâmetros de paginação inválidos." });

            var total = await _context.VagaHabilidades.CountAsync();

            var lista = await _context.VagaHabilidades
                .Include(vh => vh.Vaga)
                .Include(vh => vh.Habilidade)
                .OrderBy(vh => vh.IdVagaHabilidade)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(vh => new
                {
                    vh.IdVagaHabilidade,
                    Vaga = vh.Vaga != null ? vh.Vaga.Titulo : "Vaga não definida",
                    Habilidade = vh.Habilidade != null ? vh.Habilidade.Nome : "Habilidade não definida"
                })
                .ToListAsync();

            var result = new
            {
                totalItems = total,
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize),
                data = lista,
                links = new List<object>
                {
                    new { rel = "self", href = GetPageUrl(page, pageSize), method = "GET" },
                    new { rel = "next", href = GetPageUrl(page + 1, pageSize), method = "GET" },
                    new { rel = "previous", href = GetPageUrl(page - 1, pageSize), method = "GET" }
                }
            };

            return Ok(result);
        }

        // GET: api/v1/vagahabilidade/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtém uma relação Vaga-Habilidade específica")]
        [SwaggerResponse(StatusCodes.Status200OK, "Relação encontrada com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Relacionamento não encontrado")]
        public async Task<IActionResult> GetById(int id)
        {
            var vh = await _context.VagaHabilidades
                .Include(v => v.Vaga)
                .Include(h => h.Habilidade)
                .FirstOrDefaultAsync(x => x.IdVagaHabilidade == id);

            if (vh == null)
                return NotFound(new { mensagem = "Relacionamento não encontrado." });

            var result = new
            {
                vh.IdVagaHabilidade,
                Vaga = vh.Vaga?.Titulo ?? "Vaga não definida",
                Habilidade = vh.Habilidade?.Nome ?? "Habilidade não definida",
                links = new List<object>
                {
                    new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
                }
            };

            return Ok(result);
        }

        // POST: api/v1/vagahabilidade
        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova relação Vaga-Habilidade")]
        [SwaggerResponse(StatusCodes.Status201Created, "Relação criada com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos")]
        public async Task<IActionResult> Create([FromBody] VagaHabilidade nova)
        {
            if (nova == null || nova.VagaId == 0 || nova.HabilidadeId == 0)
                return BadRequest(new { mensagem = "Dados inválidos." });

            _context.VagaHabilidades.Add(nova);
            await _context.SaveChangesAsync();

            var url = GetByIdUrl(nova.IdVagaHabilidade);

            var result = new
            {
                nova.IdVagaHabilidade,
                nova.VagaId,
                nova.HabilidadeId,
                links = new List<object>
                {
                    new { rel = "self", href = url, method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
                }
            };

            return Created(url, result);
        }

        // PUT: api/v1/vagahabilidade/{id}
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza uma relação Vaga-Habilidade")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Relação atualizada com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Relacionamento não encontrado")]
        public async Task<IActionResult> Update(int id, [FromBody] VagaHabilidade atualizada)
        {
            if (atualizada == null || atualizada.VagaId == 0 || atualizada.HabilidadeId == 0)
                return BadRequest(new { mensagem = "Dados inválidos." });

            var vh = await _context.VagaHabilidades.FindAsync(id);
            if (vh == null)
                return NotFound(new { mensagem = "Relacionamento não encontrado." });

            vh.VagaId = atualizada.VagaId;
            vh.HabilidadeId = atualizada.HabilidadeId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/v1/vagahabilidade/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Remove uma relação Vaga-Habilidade")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Relação removida com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Relacionamento não encontrado")]
        public async Task<IActionResult> Delete(int id)
        {
            var vh = await _context.VagaHabilidades.FindAsync(id);
            if (vh == null)
                return NotFound(new { mensagem = "Relacionamento não encontrado." });

            _context.VagaHabilidades.Remove(vh);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // MÉTODOS AUXILIARES HATEOAS
        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), "VagaHabilidade", new { id }) ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetAll), "VagaHabilidade", new { page, pageSize }) ?? string.Empty;
    }
}
