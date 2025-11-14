using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;

namespace JobFitScoreAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/empresa")]
    [ApiVersion("2.0")]
    public class EmpresaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmpresaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? nome, string? cnpj)
        {
            var query = _context.Empresas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(e => e.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(cnpj))
                query = query.Where(e => e.Cnpj.Contains(cnpj));

            var result = await query
                .Select(e => new
                {
                    e.IdEmpresa,
                    e.Nome,
                    e.Cnpj,
                    e.Email
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
