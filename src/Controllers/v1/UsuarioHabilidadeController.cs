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

        // ============================================================
        // GET: api/v1/usuariohabilidade
        // ============================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioHabilidade>>> GetAll()
        {
            var usuarioHabilidades = await _context.UsuarioHabilidades
                .Include(u => u.Usuario)
                .Include(h => h.Habilidade)
                .AsNoTracking()
                .ToListAsync();

            return Ok(usuarioHabilidades);
        }

        // ============================================================
        // POST: api/v1/usuariohabilidade
        // ============================================================
        [HttpPost]
        public async Task<ActionResult<UsuarioHabilidade>> Create([FromBody] UsuarioHabilidade usuarioHabilidade)
        {
            if (usuarioHabilidade == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

            _context.UsuarioHabilidades.Add(usuarioHabilidade);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), usuarioHabilidade);
        }

        // ============================================================
        // DELETE: api/v1/usuariohabilidade/{id}
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioHabilidade = await _context.UsuarioHabilidades.FindAsync(id);
            if (usuarioHabilidade == null)
                return NotFound(new { mensagem = "Registro não encontrado." });

            _context.UsuarioHabilidades.Remove(usuarioHabilidade);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
