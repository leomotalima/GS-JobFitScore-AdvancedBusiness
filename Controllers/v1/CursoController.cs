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
    public class CursoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public CursoController(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        // ============================================================
        // GET: api/v1/curso?page=1&pageSize=10
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { mensagem = "Parâmetros de paginação inválidos." });

            var total = await _context.Cursos.CountAsync();

            var cursos = await _context.Cursos
                .OrderBy(c => c.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.IdCurso,
                    c.Nome,
                    c.Descricao,
                    c.CargaHoraria
                })
                .ToListAsync();

            var result = new
            {
                totalItems = total,
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize),
                data = cursos,
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
        // GET: api/v1/curso/{id}
        // ============================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
                return NotFound(new { mensagem = "Curso não encontrado." });

            var result = new
            {
                curso.IdCurso,
                curso.Nome,
                curso.Descricao,
                curso.CargaHoraria,
                links = new List<object>
                {
                    new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
                }
            };

            return Ok(result);
        }

        // ============================================================
        // POST: api/v1/curso
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Curso curso)
        {
            if (curso == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var url = GetByIdUrl(curso.IdCurso);

            var result = new
            {
                curso.IdCurso,
                curso.Nome,
                curso.Descricao,
                curso.CargaHoraria,
                links = new List<object>
                {
                    new { rel = "self", href = url, method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
                }
            };

            return Created(url, result);
        }

        // ============================================================
        // Métodos auxiliares (HATEOAS)
        // ============================================================
        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(
                HttpContext,
                action: nameof(GetById),
                controller: "Curso",
                values: new { id }
            ) ?? string.Empty; 

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(
                HttpContext,
                action: nameof(GetAll),
                controller: "Curso",
                values: new { page, pageSize }
            ) ?? string.Empty; 
    }
}
