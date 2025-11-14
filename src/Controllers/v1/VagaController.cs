using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Dtos.Vaga;

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
                .Select(v => new VagaOutput
                {
                    IdVaga = v.IdVaga,
                    Titulo = v.Titulo,
                    Descricao = v.Descricao,
                    NivelExperiencia = v.NivelExperiencia,
                    Salario = v.Salario,
                    Localizacao = v.Localizacao
                })
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

            var result = new VagaOutput
            {
                IdVaga = vaga.IdVaga,
                Titulo = vaga.Titulo,
                Descricao = vaga.Descricao,
                NivelExperiencia = vaga.NivelExperiencia,
                Salario = vaga.Salario,
                Localizacao = vaga.Localizacao
            };

            return Ok(new
            {
                result,
                links = GenerateLinks(id)
            });
        }

        // ============================================================
        // POST: api/v1/vaga
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VagaInput input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vaga = new Vaga
            {
                Titulo = input.Titulo ?? string.Empty,
                Descricao = input.Descricao ?? string.Empty,
                NivelExperiencia = input.NivelExperiencia ?? string.Empty,
                Salario = input.Salario ?? 0,
                Localizacao = input.Localizacao ?? string.Empty
            };

            _context.Vagas.Add(vaga);
            await _context.SaveChangesAsync();

            var url = GetByIdUrl(vaga.IdVaga);

            return Created(url, new
            {
                result = new VagaOutput
                {
                    IdVaga = vaga.IdVaga,
                    Titulo = vaga.Titulo,
                    Descricao = vaga.Descricao,
                    NivelExperiencia = vaga.NivelExperiencia,
                    Salario = vaga.Salario,
                    Localizacao = vaga.Localizacao
                },
                links = GenerateLinks(vaga.IdVaga)
            });
        }

        // ============================================================
        // PUT: api/v1/vaga/{id}
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VagaUpdateInput input)
        {
            var vaga = await _context.Vagas.FindAsync(id);
            if (vaga == null)
                return NotFound(new { mensagem = "Vaga não encontrada." });

            vaga.Titulo = !string.IsNullOrWhiteSpace(input.Titulo) ? input.Titulo! : vaga.Titulo;
            vaga.Descricao = !string.IsNullOrWhiteSpace(input.Descricao) ? input.Descricao! : vaga.Descricao;
            vaga.NivelExperiencia = !string.IsNullOrWhiteSpace(input.NivelExperiencia) ? input.NivelExperiencia! : vaga.NivelExperiencia;
            vaga.Localizacao = !string.IsNullOrWhiteSpace(input.Localizacao) ? input.Localizacao! : vaga.Localizacao;

            if (input.Salario.HasValue && input.Salario > 0)
                vaga.Salario = input.Salario.Value;

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
        // HATEOAS HELPERS
        // ============================================================
        private IEnumerable<object> GenerateLinks(int id) =>
            new List<object>
            {
                new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                new { rel = "update", href = GetByIdUrl(id), method = "PUT" },
                new { rel = "delete", href = GetByIdUrl(id), method = "DELETE" },
                new { rel = "all", href = GetPageUrl(1, 5), method = "GET" }
            };

        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), "Vaga", new { id }) ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetAll), "Vaga", new { page, pageSize }) ?? string.Empty;
    }
}
