using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;

namespace JobFitScoreAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/habilidade")]
    [ApiVersion("2.0")]
    public class HabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HabilidadeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? nome)
        {
            var result = await _context.Habilidades
                .Where(h => string.IsNullOrEmpty(nome) || h.Nome.Contains(nome))
                .Select(h => new
                {
                    h.IdHabilidade,
                    h.Nome,
                    h.Descricao
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
