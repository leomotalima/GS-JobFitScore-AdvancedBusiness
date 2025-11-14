using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;

namespace JobFitScoreAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/vagahabilidade")]
    [ApiVersion("2.0")]
    public class VagaHabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VagaHabilidadeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("vaga/{vagaId}")]
        public async Task<IActionResult> GetByVaga(int vagaId)
        {
            var result = await _context.VagaHabilidades
                .Where(vh => vh.VagaId == vagaId)
                .Select(vh => new
                {
                    vh.IdVagaHabilidade,
                    vh.VagaId,
                    Habilidade = vh.Habilidade!.Nome
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
