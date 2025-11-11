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
    public class UsuarioHabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioHabilidadeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioHabilidade>>> GetAll()
        {
            return await _context.UsuarioHabilidades
                .Include(u => u.Usuario)
                .Include(h => h.Habilidade)
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioHabilidade>> Create(UsuarioHabilidade usuarioHabilidade)
        {
            _context.UsuarioHabilidades.Add(usuarioHabilidade);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), usuarioHabilidade);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.UsuarioHabilidades.FindAsync(id);
            if (item == null) return NotFound();

            _context.UsuarioHabilidades.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
