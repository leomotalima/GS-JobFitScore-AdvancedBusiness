using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/candidaturas")]
    public class CandidaturaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CandidaturaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/candidaturas
        [HttpGet]
        public async Task<IActionResult> GetCandidaturas()
        {
            var candidaturas = await _context.Candidaturas
                .Include(c => c.Vaga)
                .Include(c => c.Usuario)
                .Select(c => new
                {
                    c.IdCandidatura,
                    Usuario = new
                    {
                        c.Usuario!.IdUsuario,
                        c.Usuario!.Nome,
                        Email = c.Usuario!.Email ?? string.Empty
                    },
                    Vaga = new
                    {
                        c.Vaga!.IdVaga,
                        c.Vaga!.Titulo
                    },
                    c.DataCandidatura,
                    c.Status
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Candidaturas listadas com sucesso.",
                data = candidaturas
            });
        }

        // GET: api/v1/candidaturas/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCandidatura(int id)
        {
            var candidatura = await _context.Candidaturas
                .Include(c => c.Vaga)
                .Include(c => c.Usuario)
                .Where(c => c.IdCandidatura == id)
                .Select(c => new
                {
                    c.IdCandidatura,
                    Usuario = new
                    {
                        c.Usuario!.IdUsuario,
                        c.Usuario!.Nome,
                        Email = c.Usuario!.Email ?? string.Empty
                    },
                    Vaga = new
                    {
                        c.Vaga!.IdVaga,
                        c.Vaga!.Titulo
                    },
                    c.DataCandidatura,
                    c.Status
                })
                .FirstOrDefaultAsync();

            if (candidatura == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Candidatura não encontrada."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Candidatura encontrada com sucesso.",
                data = candidatura
            });
        }

        // POST: api/v1/candidaturas
        [HttpPost]
        public async Task<IActionResult> CreateCandidatura([FromBody] Candidatura candidatura)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Candidaturas.Add(candidatura);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Candidatura criada com sucesso.",
                data = candidatura
            });
        }

        // PUT: api/v1/candidaturas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCandidatura(int id, [FromBody] Candidatura candidatura)
        {
            var existente = await _context.Candidaturas.FindAsync(id);

            if (existente == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Candidatura não encontrada."
                });
            }

            existente.UsuarioId = candidatura.UsuarioId;
            existente.VagaId = candidatura.VagaId;
            existente.Status = candidatura.Status ?? existente.Status;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Candidatura atualizada com sucesso.",
                data = existente
            });
        }

        // DELETE: api/v1/candidaturas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidatura(int id)
        {
            var candidatura = await _context.Candidaturas.FindAsync(id);

            if (candidatura == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Candidatura não encontrada."
                });
            }

            _context.Candidaturas.Remove(candidatura);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Candidatura removida com sucesso."
            });
        }
    }
}
