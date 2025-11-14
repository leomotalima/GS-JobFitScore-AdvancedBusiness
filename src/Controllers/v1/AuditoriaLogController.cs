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
    public class AuditoriaLogController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public AuditoriaLogController(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        // =====================================================
        // GET: api/v1/AuditoriaLog?page=1&pageSize=10
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { mensagem = "Parâmetros de paginação inválidos." });

            var total = await _context.AuditoriaLogs.CountAsync();

            var logs = await _context.AuditoriaLogs
                .OrderByDescending(a => a.DataOperacao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            var result = new
            {
                totalItems = total,
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize),
                data = logs,
                links = new List<object>
                {
                    new { rel = "self", href = GetPageUrl(page, pageSize), method = "GET" },
                    new { rel = "next", href = GetPageUrl(page + 1, pageSize), method = "GET" },
                    new { rel = "previous", href = GetPageUrl(page - 1, pageSize), method = "GET" }
                }
            };

            return Ok(result);
        }

        // =====================================================
        // GET: api/v1/AuditoriaLog/{id}
        // =====================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await _context.AuditoriaLogs.FindAsync(id);

            if (log == null)
                return NotFound(new { mensagem = "Log não encontrado." });

            var result = new
            {
                log.IdAuditoria,
                log.NomeTabela,
                log.Operacao,
                log.RegistroId,
                log.UsuarioBanco,
                log.DataOperacao,
                log.Detalhe,
                links = new List<object>
                {
                    new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
                }
            };

            return Ok(result);
        }

        // =====================================================
        // Métodos auxiliares
        // =====================================================

        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(
                HttpContext,
                nameof(GetById),
                "AuditoriaLog",
                new { id }
            ) ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(
                HttpContext,
                nameof(GetAll),
                "AuditoriaLog",
                new { page, pageSize }
            ) ?? string.Empty;
    }
}
