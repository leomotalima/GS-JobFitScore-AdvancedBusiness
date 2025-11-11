using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Asp.Versioning.ApiVersion(1.0)]
    public class HabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HabilidadeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/habilidade
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Habilidade>>> GetAll()
        {
            return await _context.Habilidades.AsNoTracking().ToListAsync();
        }

        // GET: api/v1/habilidade/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Habilidade>> GetById(int id)
        {
            var habilidade = await _context.Habilidades.FindAsync(id);
            if (habilidade == null) return NotFound();
            return Ok(habilidade);
        }

        // POST: api/v1/habilidade
        [HttpPost]
        public async Task<ActionResult<Habilidade>> Create(Habilidade habilidade)
        {
            _context.Habilidades.Add(habilidade);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = habilidade.IdHabilidade }, habilidade);
        }

        // PUT: api/v1/habilidade/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Habilidade habilidade)
        {
            if (id != habilidade.IdHabilidade) return BadRequest();
            _context.Entry(habilidade).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/v1/habilidade/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var habilidade = await _context.Habilidades.FindAsync(id);
            if (habilidade == null) return NotFound();

            _context.Habilidades.Remove(habilidade);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
