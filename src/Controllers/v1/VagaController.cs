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
    public class VagaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public VagaController(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        // ============================================================
        // GET: api/v1/vaga?page=1&pageSize=5
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 5)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { mensagem = "Parâmetros de paginação inválidos." });

            var total = await _context.Vagas.CountAsync();

            var vagas = await _context.Vagas
                .OrderBy(v => v.IdVaga)
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
                data = vagas,
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
        // GET: api/v1/vaga/{id}
        // ============================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vaga = await _context.Vagas.FindAsync(id);
            if (vaga == null)
                return NotFound(new { mensagem = "Vaga não encontrada." });

            var result = new
            {
                vaga.IdVaga,
                vaga.Titulo,
                vaga.Descricao,
                vaga.NivelExperiencia,
                vaga.Salario,
                vaga.Localizacao,
                links = new List<object>
                {
                    new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                    new { rel = "update", href = GetByIdUrl(id), method = "PUT" },
                    new { rel = "delete", href = GetByIdUrl(id), method = "DELETE" },
                    new { rel = "all", href = GetPageUrl(1, 5), method = "GET" }
                }
            };

            return Ok(result);
        }

        // ============================================================
        // POST: api/v1/vaga
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Vaga vaga)
        {
            if (vaga == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

            _context.Vagas.Add(vaga);
            await _context.SaveChangesAsync();

            var url = GetByIdUrl(vaga.IdVaga);

            var result = new
            {
                vaga.IdVaga,
                vaga.Titulo,
                vaga.Descricao,
                vaga.NivelExperiencia,
                vaga.Salario,
                vaga.Localizacao,
                links = new List<object>
                {
                    new { rel = "self", href = url, method = "GET" },
                    new { rel = "update", href = url, method = "PUT" },
                    new { rel = "delete", href = url, method = "DELETE" },
                    new { rel = "all", href = GetPageUrl(1, 5), method = "GET" }
                }
            };

            return Created(url, result);
        }

        // ============================================================
        // PUT: api/v1/vaga/{id}
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Vaga vagaAtualizada)
        {
            if (vagaAtualizada == null || id != vagaAtualizada.IdVaga)
                return BadRequest(new { mensagem = "O ID da URL não corresponde ao corpo da requisição." });

            var vaga = await _context.Vagas.FindAsync(id);
            if (vaga == null)
                return NotFound(new { mensagem = "Vaga não encontrada." });

            vaga.Titulo = vagaAtualizada.Titulo;
            vaga.Descricao = vagaAtualizada.Descricao;
            vaga.NivelExperiencia = vagaAtualizada.NivelExperiencia;
            vaga.Salario = vagaAtualizada.Salario;
            vaga.Localizacao = vagaAtualizada.Localizacao;

            _context.Entry(vaga).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // DELETE: api/v1/vaga/{id}
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var vaga = await _context.Vagas.FindAsync(id);
            if (vaga == null)
                return NotFound(new { mensagem = "Vaga não encontrada." });

            _context.Vagas.Remove(vaga);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // Métodos auxiliares HATEOAS
        // ============================================================
        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(
                HttpContext,
                action: nameof(GetById),
                controller: "Vaga",
                values: new { id }
            ) ?? string.Empty; 

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(
                HttpContext,
                action: nameof(GetAll),
                controller: "Vaga",
                values: new { page, pageSize }
            ) ?? string.Empty; 
    }
}
