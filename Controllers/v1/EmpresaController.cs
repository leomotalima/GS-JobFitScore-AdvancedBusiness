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
    public class EmpresaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmpresaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/empresa
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Empresa>>> GetAll()
        {
            return await _context.Empresas.AsNoTracking().ToListAsync();
        }

        // GET: api/v1/empresa/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Empresa>> GetById(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null) return NotFound();
            return Ok(empresa);
        }

        // POST: api/v1/empresa
        [HttpPost]
        public async Task<ActionResult<Empresa>> Create(Empresa empresa)
        {
            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = empresa.IdEmpresa }, empresa);
        }

        // PUT: api/v1/empresa/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Empresa empresa)
        {
            if (id != empresa.IdEmpresa) return BadRequest();
            _context.Entry(empresa).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/v1/empresa/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null) return NotFound();

            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
