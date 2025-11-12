using Asp.Versioning;
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
        // Lista paginada de empresas
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
        // Busca uma empresa pelo ID
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
        // Cria uma nova empresa
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmpresaInput input)
        {
            if (input == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

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

            var result = new
            {
                empresa.IdEmpresa,
                empresa.Nome,
                empresa.Cnpj,
                empresa.Email,
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
        // PUT: api/v1/empresa/{id}
        // Atualiza uma empresa
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmpresaInput input)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null)
                return NotFound(new { mensagem = "Empresa não encontrada." });

            empresa.Nome = input.Nome;
            empresa.Email = input.Email;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // DELETE: api/v1/empresa/{id}
        // Remove uma empresa
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
        // Métodos auxiliares HATEOAS
        // ============================================================
        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), "Empresa", new { id })
            ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetAll), "Empresa", new { page, pageSize })
            ?? string.Empty;
    }
}
