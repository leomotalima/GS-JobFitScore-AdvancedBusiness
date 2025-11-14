using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CandidaturaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public CandidaturaController(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        // ============================================================
        // GET: api/v1/candidatura?page=1&pageSize=5
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 5)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { mensagem = "Parâmetros de paginação inválidos." });

            var total = await _context.Candidaturas.CountAsync();

            var candidaturas = await _context.Candidaturas
                .Include(c => c.Usuario)
                .Include(c => c.Vaga)
                .OrderByDescending(c => c.DataCandidatura)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.IdCandidatura,
                    Usuario = c.Usuario != null ? c.Usuario.Nome : "Usuário não definido",
                    Vaga = c.Vaga != null ? c.Vaga.Titulo : "Vaga não definida",
                    c.Score,
                    c.DataCandidatura
                })
                .ToListAsync();

            var result = new
            {
                totalItems = total,
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize),
                data = candidaturas,
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
        // GET: api/v1/candidatura/{id}
        // ============================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var candidatura = await _context.Candidaturas
                .Include(c => c.Usuario)
                .Include(c => c.Vaga)
                .FirstOrDefaultAsync(c => c.IdCandidatura == id);

            if (candidatura == null)
                return NotFound(new { mensagem = "Candidatura não encontrada." });

            var result = new
            {
                candidatura.IdCandidatura,
                Usuario = candidatura.Usuario?.Nome ?? "Usuário não definido",
                Vaga = candidatura.Vaga?.Titulo ?? "Vaga não definida",
                candidatura.Score,
                candidatura.DataCandidatura,
                links = new List<object>
                {
                    new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 5), method = "GET" }
                }
            };

            return Ok(result);
        }

        // ============================================================
        // POST: api/v1/candidatura
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Candidatura candidatura)
        {
            if (candidatura == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

            _context.Candidaturas.Add(candidatura);
            await _context.SaveChangesAsync();

            var url = GetByIdUrl(candidatura.IdCandidatura);

            var result = new
            {
                candidatura.IdCandidatura,
                candidatura.Score,
                candidatura.DataCandidatura,
                links = new List<object>
                {
                    new { rel = "self", href = url, method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 5), method = "GET" }
                }
            };

            return Created(url, result);
        }

        // ============================================================
        // Métodos auxiliares para gerar URLs dinâmicas (HATEOAS)
        // ============================================================
        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(
                HttpContext,
                action: nameof(GetById),
                controller: "Candidatura",
                values: new { id }
            ) ?? string.Empty; 

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(
                HttpContext,
                action: nameof(GetAll),
                controller: "Candidatura",
                values: new { page, pageSize }
            ) ?? string.Empty; 
    }
}
