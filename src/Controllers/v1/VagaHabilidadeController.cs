using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class VagaHabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public VagaHabilidadeController(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        // ============================================================
        // GET: api/v1/vagahabilidade?page=1&pageSize=10
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { mensagem = "Parâmetros de paginação inválidos." });

            var total = await _context.VagaHabilidades.CountAsync();

            var dados = await _context.VagaHabilidades
                .Include(v => v.Vaga)
                .Include(h => h.Habilidade)
                .OrderBy(vh => vh.IdVagaHabilidade)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Select(vh => new
                {
                    vh.IdVagaHabilidade,
                    Vaga = vh.Vaga != null ? vh.Vaga.Titulo : "Vaga não encontrada",
                    Habilidade = vh.Habilidade != null ? vh.Habilidade.Nome : "Habilidade não encontrada",
                    vh.VagaId,
                    vh.HabilidadeId
                })
                .ToListAsync();

            var result = new
            {
                totalItems = total,
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize),
                data = dados,
                links = new List<object>
                {
                    new { rel = "self", href = GetPageUrl(page, pageSize), method = "GET" },
                    new { rel = "next", href = GetPageUrl(page + 1, pageSize), method = "GET" },
                    new { rel = "previous", href = GetPageUrl(page - 1, pageSize), method = "GET" }
                }
            };

            return Ok(result);
        }

        // ============================================================
        // GET: api/v1/vagahabilidade/{id}
        // ============================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var registro = await _context.VagaHabilidades
                .Include(v => v.Vaga)
                .Include(h => h.Habilidade)
                .FirstOrDefaultAsync(x => x.IdVagaHabilidade == id);

            if (registro == null)
                return NotFound(new { mensagem = "Registro não encontrado." });

            var result = new
            {
                registro.IdVagaHabilidade,
                Vaga = registro.Vaga?.Titulo ?? "Vaga não encontrada",
                Habilidade = registro.Habilidade?.Nome ?? "Habilidade não encontrada",
                registro.VagaId,
                registro.HabilidadeId,
                links = GenerateLinks(id)
            };

            return Ok(result);
        }

        // ============================================================
        // POST: api/v1/vagahabilidade
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VagaHabilidade input)
        {
            if (input == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

            
            var vaga = await _context.Vagas.FindAsync(input.VagaId);
            if (vaga == null)
                return NotFound(new { mensagem = "Vaga não encontrada." });

            
            var habilidade = await _context.Habilidades.FindAsync(input.HabilidadeId);
            if (habilidade == null)
                return NotFound(new { mensagem = "Habilidade não encontrada." });

            
            bool existe = await _context.VagaHabilidades
                .AnyAsync(x => x.VagaId == input.VagaId && x.HabilidadeId == input.HabilidadeId);

            if (existe)
                return Conflict(new { mensagem = "Essa habilidade já está vinculada a essa vaga." });

            _context.VagaHabilidades.Add(input);
            await _context.SaveChangesAsync();

            var url = GetByIdUrl(input.IdVagaHabilidade);

            return Created(url, new
            {
                input.IdVagaHabilidade,
                Vaga = vaga.Titulo,
                Habilidade = habilidade.Nome,
                links = GenerateLinks(input.IdVagaHabilidade)
            });
        }

        // ============================================================
        // DELETE: api/v1/vagahabilidade/{id}
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var registro = await _context.VagaHabilidades.FindAsync(id);
            if (registro == null)
                return NotFound(new { mensagem = "Registro não encontrado." });

            _context.VagaHabilidades.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // MÉTODOS AUXILIARES (HATEOAS)
        // ============================================================
        private IEnumerable<object> GenerateLinks(int id) =>
            new List<object>
            {
                new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                new { rel = "delete", href = GetByIdUrl(id), method = "DELETE" },
                new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
            };

        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), "VagaHabilidade", new { id })
            ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetAll), "VagaHabilidade", new { page, pageSize })
            ?? string.Empty;
    }
}
