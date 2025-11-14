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
    public class UsuarioHabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public UsuarioHabilidadeController(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        // DTO de entrada
        public class UsuarioHabilidadeInput
        {
            public int UsuarioId { get; set; }
            public int HabilidadeId { get; set; }
        }

        // ============================================================
        // GET: api/v1/usuariohabilidade?page=1&pageSize=10
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { mensagem = "Parâmetros de paginação inválidos." });

            var total = await _context.UsuarioHabilidades.CountAsync();

            var registros = await _context.UsuarioHabilidades
                .Include(u => u.Usuario)
                .Include(h => h.Habilidade)
                .OrderBy(u => u.IdUsuarioHabilidade)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Select(x => new
                {
                    x.IdUsuarioHabilidade,
                    Usuario = x.Usuario != null ? x.Usuario.Nome : null,
                    Habilidade = x.Habilidade != null ? x.Habilidade.Nome : null
                })
                .ToListAsync();

            var result = new
            {
                totalItems = total,
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize),
                data = registros,
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
        // POST: api/v1/usuariohabilidade
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioHabilidadeInput input)
        {
            if (input == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

            // valida usuário
            var usuario = await _context.Usuarios.FindAsync(input.UsuarioId);
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            // valida habilidade
            var habilidade = await _context.Habilidades.FindAsync(input.HabilidadeId);
            if (habilidade == null)
                return NotFound(new { mensagem = "Habilidade não encontrada." });

            // verifica duplicidade
            bool existe = await _context.UsuarioHabilidades
                .AnyAsync(x => x.UsuarioId == input.UsuarioId && x.HabilidadeId == input.HabilidadeId);

            if (existe)
                return Conflict(new { mensagem = "Usuário já possui esta habilidade cadastrada." });

            var novo = new UsuarioHabilidade
            {
                UsuarioId = input.UsuarioId,
                HabilidadeId = input.HabilidadeId
            };

            _context.UsuarioHabilidades.Add(novo);
            await _context.SaveChangesAsync();

            var url = GetByIdUrl(novo.IdUsuarioHabilidade);

            return Created(url, new
            {
                novo.IdUsuarioHabilidade,
                Usuario = usuario.Nome,
                Habilidade = habilidade.Nome,
                links = new List<object>
                {
                    new { rel = "self", href = url, method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
                }
            });
        }

        // ============================================================
        // GET: api/v1/usuariohabilidade/{id}
        // ============================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var registro = await _context.UsuarioHabilidades
                .Include(u => u.Usuario)
                .Include(h => h.Habilidade)
                .FirstOrDefaultAsync(x => x.IdUsuarioHabilidade == id);

            if (registro == null)
                return NotFound(new { mensagem = "Registro não encontrado." });

            var result = new
            {
                registro.IdUsuarioHabilidade,
                Usuario = registro.Usuario != null ? registro.Usuario.Nome : null,
                Habilidade = registro.Habilidade != null ? registro.Habilidade.Nome : null,
                links = new List<object>
                {
                    new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                    new { rel = "delete", href = GetByIdUrl(id), method = "DELETE" },
                    new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
                }
            };

            return Ok(result);
        }

        // ============================================================
        // DELETE: api/v1/usuariohabilidade/{id}
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var registro = await _context.UsuarioHabilidades.FindAsync(id);
            if (registro == null)
                return NotFound(new { mensagem = "Registro não encontrado." });

            _context.UsuarioHabilidades.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // MÉTODOS AUXILIARES
        // ============================================================
        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), "UsuarioHabilidade", new { id })
            ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetAll), "UsuarioHabilidade", new { page, pageSize })
            ?? string.Empty;
    }
}
