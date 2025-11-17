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
    public class HabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public HabilidadeController(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

       
        // GET: api/v1/habilidade?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { mensagem = "Parâmetros de paginação inválidos." });

            var total = await _context.Habilidades.CountAsync();

            var habilidades = await _context.Habilidades
                .OrderBy(h => h.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(h => new
                {
                    h.IdHabilidade,
                    h.Nome
                })
                .ToListAsync();

            var result = new
            {
                totalItems = total,
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize),
                data = habilidades,
                links = new List<object>
                {
                    new { rel = "self", href = GetPageUrl(page, pageSize), method = "GET" },
                    new { rel = "next", href = GetPageUrl(page + 1, pageSize), method = "GET" },
                    new { rel = "previous", href = GetPageUrl(page - 1, pageSize), method = "GET" }
                }
            };

            return Ok(result);
        }

        
        // GET: api/v1/habilidade/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var habilidade = await _context.Habilidades.FindAsync(id);

            if (habilidade == null)
                return NotFound(new { mensagem = "Habilidade não encontrada." });

            var result = new
            {
                habilidade.IdHabilidade,
                habilidade.Nome,
                links = new List<object>
                {
                    new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
                }
            };

            return Ok(result);
        }

       
        // POST: api/v1/habilidade
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Habilidade habilidade)
        {
            if (habilidade == null || string.IsNullOrWhiteSpace(habilidade.Nome))
                return BadRequest(new { mensagem = "Dados inválidos." });

            _context.Habilidades.Add(habilidade);
            await _context.SaveChangesAsync();

            var url = GetByIdUrl(habilidade.IdHabilidade);

            var result = new
            {
                habilidade.IdHabilidade,
                habilidade.Nome,
                links = new List<object>
                {
                    new { rel = "self", href = url, method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
                }
            };

            return Created(url, result);
        }

      
        // PUT: api/v1/habilidade/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Habilidade atualizada)
        {
            if (atualizada == null || string.IsNullOrWhiteSpace(atualizada.Nome))
                return BadRequest(new { mensagem = "Dados inválidos." });

            var habilidade = await _context.Habilidades.FindAsync(id);
            if (habilidade == null)
                return NotFound(new { mensagem = "Habilidade não encontrada." });

            habilidade.Nome = atualizada.Nome;

            await _context.SaveChangesAsync();

            return NoContent();
        }

       
        // DELETE: api/v1/habilidade/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var habilidade = await _context.Habilidades.FindAsync(id);
            if (habilidade == null)
                return NotFound(new { mensagem = "Habilidade não encontrada." });

            _context.Habilidades.Remove(habilidade);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        // MÉTODOS AUXILIARES HATEOAS
        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), "Habilidade", new { id }) ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetAll), "Habilidade", new { page, pageSize }) ?? string.Empty;
    }
}
