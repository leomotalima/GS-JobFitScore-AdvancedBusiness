using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Dtos.Empresa;
using BCrypt.Net;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class EmpresaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public EmpresaController(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        // ============================================================
        // GET: api/v1/empresa?page=1&pageSize=5
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 5)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { mensagem = "Parâmetros de paginação inválidos." });

            var total = await _context.Empresas.CountAsync();

            var empresas = await _context.Empresas
                .OrderBy(e => e.IdEmpresa)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Select(e => new EmpresaOutput
                {
                    IdEmpresa = e.IdEmpresa,
                    Nome = e.Nome,
                    Cnpj = e.Cnpj,
                    Email = e.Email
                })
                .ToListAsync();

            var result = new
            {
                totalItems = total,
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize),
                data = empresas,
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
        // GET: api/v1/empresa/{id}
        // ============================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null)
                return NotFound(new { mensagem = "Empresa não encontrada." });

            var result = new
            {
                empresa.IdEmpresa,
                empresa.Nome,
                empresa.Cnpj,
                empresa.Email,
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
        // POST: api/v1/empresa
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmpresaInput input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Empresas.AnyAsync(e => e.Cnpj == input.Cnpj))
                return Conflict(new { mensagem = "CNPJ já cadastrado." });

            if (await _context.Empresas.AnyAsync(e => e.Email == input.Email))
                return Conflict(new { mensagem = "E-mail já cadastrado." });

            var empresa = new Empresa
            {
                Nome = input.Nome,
                Cnpj = input.Cnpj,
                Email = input.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(input.Senha!)
            };

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();

            var url = GetByIdUrl(empresa.IdEmpresa);

            var result = new EmpresaOutput
            {
                IdEmpresa = empresa.IdEmpresa,
                Nome = empresa.Nome,
                Cnpj = empresa.Cnpj,
                Email = empresa.Email
            };

            return Created(url, new { result, links = GenerateLinks(empresa.IdEmpresa) });
        }

        // ============================================================
        // PUT: api/v1/empresa/{id}
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmpresaUpdateInput input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null)
                return NotFound(new { mensagem = "Empresa não encontrada." });

            empresa.Nome = input.Nome ?? empresa.Nome;
            empresa.Email = input.Email ?? empresa.Email;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // DELETE: api/v1/empresa/{id}
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null)
                return NotFound(new { mensagem = "Empresa não encontrada." });

            _context.Empresas.Remove(empresa);
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
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), "Empresa", new { id }) ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetAll), "Empresa", new { page, pageSize }) ?? string.Empty;
    }
}
