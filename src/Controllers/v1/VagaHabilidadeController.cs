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
    public class VagaHabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VagaHabilidadeController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET: api/v1/vagahabilidade
        // ============================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VagaHabilidade>>> GetAll()
        {
            var vagaHabilidades = await _context.VagaHabilidades
                .Include(v => v.Vaga)
                .Include(h => h.Habilidade)
                .AsNoTracking()
                .ToListAsync();

            return Ok(vagaHabilidades);
        }

        // ============================================================
        // POST: api/v1/vagahabilidade
        // ============================================================
        [HttpPost]
        public async Task<ActionResult<VagaHabilidade>> Create([FromBody] VagaHabilidade vagaHabilidade)
        {
            if (vagaHabilidade == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

            _context.VagaHabilidades.Add(vagaHabilidade);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new
            {
                vagaHabilidade.IdVagaHabilidade,
                vagaHabilidade.VagaId,
                vagaHabilidade.HabilidadeId
            }, vagaHabilidade);
        }

        // ============================================================
        // DELETE: api/v1/vagahabilidade/{id}
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var vagaHabilidade = await _context.VagaHabilidades.FindAsync(id);
            if (vagaHabilidade == null)
                return NotFound(new { mensagem = "Registro não encontrado." });

            _context.VagaHabilidades.Remove(vagaHabilidade);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
