using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Dtos;
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

        // ============================================
        // GET: api/v1/empresa?page=1&pageSize=5
        // ============================================
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
                .Select(e => new EmpresaResponseDto
                {
                    IdEmpresa = e.IdEmpresa,
                    Nome = e.Nome,
                    Cnpj = e.Cnpj,
                    Email = e.Email,
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

        // ============================================
        // GET: api/v1/empresa/{id}
        // ============================================
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

        // ============================================
        // POST: api/v1/empresa
        // ============================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmpresaCreateDto dto)
        {
            if (dto == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

            if (await _context.Empresas.AnyAsync(e => e.Cnpj == dto.Cnpj))
                return Conflict(new { mensagem = "CNPJ já cadastrado." });

            if (await _context.Empresas.AnyAsync(e => e.Email == dto.Email))
                return Conflict(new { mensagem = "E-mail já cadastrado." });

            var empresa = new Empresa
            {
                Nome = dto.Nome,
                Cnpj = dto.Cnpj,
                Email = dto.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            };

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();

            var url = GetByIdUrl(empresa.IdEmpresa);

            return Created(url, new
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
            });
        }

        // ============================================
        // PUT: api/v1/empresa/{id}
        // ============================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmpresaUpdateDto dto)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null)
                return NotFound(new { mensagem = "Empresa não encontrada." });

            empresa.Nome = dto.Nome;
            empresa.Email = dto.Email;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================
        // DELETE: api/v1/empresa/{id}
        // ============================================
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

        // ============================================
        // Links auxiliares (HATEOAS)
        // ============================================
        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), "Empresa", new { id }) ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetAll), "Empresa", new { page, pageSize }) ?? string.Empty;
    }
}
