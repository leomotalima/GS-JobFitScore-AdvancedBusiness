using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
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
        public async Task<IActionResult> GetAll()
        {
            var dados = await _context.VagaHabilidades
                .Include(v => v.Vaga)
                .Include(h => h.Habilidade)
                .AsNoTracking()
                .Select(vh => new
                {
                    vh.IdVagaHabilidade,
                    Vaga = vh.Vaga != null ? vh.Vaga.Titulo : "Vaga não encontrada",
                    Habilidade = vh.Habilidade != null ? vh.Habilidade.Nome : "Habilidade não encontrada",
                    vh.VagaId,
                    vh.HabilidadeId
                })
                .ToListAsync();

            return Ok(dados);
        }

        // ============================================================
        // POST: api/v1/vagahabilidade
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VagaHabilidade input)
        {
            if (input == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

            // Validação: vaga existe?
            var vaga = await _context.Vagas.FindAsync(input.VagaId);
            if (vaga == null)
                return NotFound(new { mensagem = "Vaga não encontrada." });

            // Validação: habilidade existe?
            var habilidade = await _context.Habilidades.FindAsync(input.HabilidadeId);
            if (habilidade == null)
                return NotFound(new { mensagem = "Habilidade não encontrada." });

            // Bloqueia duplicados (vaga + habilidade já existe)
            bool existe = await _context.VagaHabilidades.AnyAsync(vh =>
                vh.VagaId == input.VagaId &&
                vh.HabilidadeId == input.HabilidadeId
            );

            if (existe)
                return Conflict(new { mensagem = "Essa habilidade já está vinculada a essa vaga." });

            _context.VagaHabilidades.Add(input);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new
            {
                input.IdVagaHabilidade,
                input.VagaId,
                input.HabilidadeId
            }, input);
        }

        // ============================================================
        // DELETE: api/v1/vagahabilidade/{id}
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var registro = await _context.VagaHabilidades.FindAsync(id);
            if (registro == null)
                return NotFound(new { mensagem = "Registro não encontrado." });

            _context.VagaHabilidades.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
