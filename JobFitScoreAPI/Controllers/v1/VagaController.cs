using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/vagas")]
    public class VagaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VagaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/vagas
        [HttpGet]
        public async Task<IActionResult> GetVagas()
        {
            var vagas = await _context.Vagas
                .Include(v => v.Empresa)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Vagas listadas com sucesso.",
                data = vagas
            });
        }

        // GET: api/v1/vagas/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVaga(int id)
        {
            var vaga = await _context.Vagas
                .Include(v => v.Empresa)
                .FirstOrDefaultAsync(v => v.IdVaga == id);

            if (vaga == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Vaga não encontrada."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Vaga encontrada com sucesso.",
                data = vaga
            });
        }

        // POST: api/v1/vagas
        [HttpPost]
        public async Task<IActionResult> CreateVaga([FromBody] Vaga vaga)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Vagas.Add(vaga);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Vaga criada com sucesso.",
                data = vaga
            });
        }

        // PUT: api/v1/vagas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVaga(int id, [FromBody] Vaga vaga)
        {
            var vagaExistente = await _context.Vagas.FindAsync(id);

            if (vagaExistente == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Vaga não encontrada."
                });
            }

            vagaExistente.Titulo = vaga.Titulo;
            vagaExistente.EmpresaId = vaga.EmpresaId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Vaga atualizada com sucesso.",
                data = vagaExistente
            });
        }

        // DELETE: api/v1/vagas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVaga(int id)
        {
            var vaga = await _context.Vagas.FindAsync(id);

            if (vaga == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Vaga não encontrada."
                });
            }

            _context.Vagas.Remove(vaga);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Vaga removida com sucesso."
            });
        }
    }
}
