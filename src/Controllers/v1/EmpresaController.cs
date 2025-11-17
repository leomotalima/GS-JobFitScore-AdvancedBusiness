using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;

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

        
        // GET: api/v1/empresa?page=1&pageSize=5
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 5)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { mensagem = "Parâmetros de paginação inválidos." });

            var total = await _context.Empresas.CountAsync();

            var empresas = await _context.Empresas
                .Include(e => e.Vagas)
                .OrderBy(e => e.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new
                {
                    e.IdEmpresa,
                    e.Nome,
                    e.Cnpj,
                    e.Email,
                   
                    Vagas = (e.Vagas ?? Enumerable.Empty<Vaga>())
                        .Select(v => new { v.IdVaga, v.Titulo })
                        .ToList()
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

        
        // GET: api/v1/empresa/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var empresa = await _context.Empresas
                .Include(e => e.Vagas)
                .FirstOrDefaultAsync(e => e.IdEmpresa == id);

            if (empresa == null)
                return NotFound(new { mensagem = "Empresa não encontrada." });

            var result = new
            {
                empresa.IdEmpresa,
                empresa.Nome,
                empresa.Cnpj,
                empresa.Email,
                Vagas = (empresa.Vagas ?? Enumerable.Empty<Vaga>())
                    .Select(v => new { v.IdVaga, v.Titulo })
                    .ToList(),
                links = new List<object>
                {
                    new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                    new { rel = "all", href = GetPageUrl(1, 5), method = "GET" }
                }
            };

            return Ok(result);
        }

        
        // POST: api/v1/empresa
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Empresa empresa)
        {
            if (empresa == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

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
                    new { rel = "all", href = GetPageUrl(1, 5), method = "GET" }
                }
            };

            return Created(url, result);
        }

        
        // PUT: api/v1/empresa/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Empresa updated)
        {
            if (updated == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null)
                return NotFound(new { mensagem = "Empresa não encontrada." });

            empresa.Nome = updated.Nome ?? empresa.Nome;
            empresa.Cnpj = updated.Cnpj ?? empresa.Cnpj;
            empresa.Email = updated.Email ?? empresa.Email;
            empresa.Senha = updated.Senha ?? empresa.Senha;
            empresa.RefreshToken = updated.RefreshToken ?? empresa.RefreshToken;
            empresa.ExpiraRefreshToken = updated.ExpiraRefreshToken ?? empresa.ExpiraRefreshToken;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        // DELETE: api/v1/empresa/{id}
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

        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), "Empresa", new { id }) ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetAll), "Empresa", new { page, pageSize }) ?? string.Empty;
    }
}
