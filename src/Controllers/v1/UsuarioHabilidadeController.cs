using Microsoft.AspNetCore.Mvc;   
using Asp.Versioning;              
using Microsoft.EntityFrameworkCore; 
using JobFitScoreAPI.Data;         
using JobFitScoreAPI.Models;


namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Asp.Versioning.ApiVersion("1.0")]
    public class UsuarioHabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public UsuarioHabilidadeController(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

       
        // GET: api/v1/usuariohabilidade?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { mensagem = "Parâmetros de paginação inválidos." });

            var total = await _context.UsuarioHabilidades.CountAsync();

            var lista = await _context.UsuarioHabilidades
                .Include(uh => uh.Usuario)
                .Include(uh => uh.Habilidade)
                .OrderBy(uh => uh.IdUsuarioHabilidade)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(uh => new
                {
                    uh.IdUsuarioHabilidade,
                    Usuario = uh.Usuario != null ? uh.Usuario.Nome : "Usuário não definido",
                    Habilidade = uh.Habilidade != null ? uh.Habilidade.Nome : "Habilidade não definida"
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

       
        // GET: api/v1/usuariohabilidade/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var uh = await _context.UsuarioHabilidades
                .Include(u => u.Usuario)
                .Include(h => h.Habilidade)
                .FirstOrDefaultAsync(x => x.IdUsuarioHabilidade == id);

            if (uh == null)
                return NotFound(new { mensagem = "Relacionamento não encontrado." });

            var result = new
            {
                uh.IdUsuarioHabilidade,
                Usuario = uh.Usuario?.Nome ?? "Usuário não definido",
                Habilidade = uh.Habilidade?.Nome ?? "Habilidade não definida",
                links = new List<object>
                {
                    new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
                }
            };

            return Ok(result);
        }

        
        // POST: api/v1/usuariohabilidade
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioHabilidade nova)
        {
            if (nova == null || nova.UsuarioId == 0 || nova.HabilidadeId == 0)
                return BadRequest(new { mensagem = "Dados inválidos." });

            _context.UsuarioHabilidades.Add(nova);
            await _context.SaveChangesAsync();

            var url = GetByIdUrl(nova.IdUsuarioHabilidade);

            var result = new
            {
                nova.IdUsuarioHabilidade,
                nova.UsuarioId,
                nova.HabilidadeId,
                links = new List<object>
                {
                    new { rel = "self", href = url, method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
                }
            };

            return Created(url, result);
        }

       
        // PUT: api/v1/usuariohabilidade/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioHabilidade atualizada)
        {
            if (atualizada == null || atualizada.UsuarioId == 0 || atualizada.HabilidadeId == 0)
                return BadRequest(new { mensagem = "Dados inválidos." });

            var uh = await _context.UsuarioHabilidades.FindAsync(id);
            if (uh == null)
                return NotFound(new { mensagem = "Relacionamento não encontrado." });

            uh.UsuarioId = atualizada.UsuarioId;
            uh.HabilidadeId = atualizada.HabilidadeId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

       
        // DELETE: api/v1/usuariohabilidade/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var uh = await _context.UsuarioHabilidades.FindAsync(id);
            if (uh == null)
                return NotFound(new { mensagem = "Relacionamento não encontrado." });

            _context.UsuarioHabilidades.Remove(uh);
            await _context.SaveChangesAsync();

            return NoContent();
        }

       
        // MÉTODOS AUXILIARES HATEOAS
        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), "UsuarioHabilidade", new { id }) ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetAll), "UsuarioHabilidade", new { page, pageSize }) ?? string.Empty;
    }
}
