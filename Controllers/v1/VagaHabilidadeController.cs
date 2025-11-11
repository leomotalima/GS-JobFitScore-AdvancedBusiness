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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VagaHabilidade>>> GetAll()
        {
            return await _context.VagaHabilidades
                .Include(v => v.Vaga)
                .Include(h => h.Habilidade)
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<VagaHabilidade>> Create(VagaHabilidade vagaHabilidade)
        {
            _context.VagaHabilidades.Add(vagaHabilidade);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), vagaHabilidade);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.VagaHabilidades.FindAsync(id);
            if (item == null) return NotFound();

            _context.VagaHabilidades.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
