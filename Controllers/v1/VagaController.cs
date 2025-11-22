using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/vagas")]
    [Tags("Vagas")]
    [Produces("application/json")]
    public class VagaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VagaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/vagas
        [HttpGet]
        [SwaggerOperation(Summary = "Lista todas as vagas")]
        [SwaggerResponse(StatusCodes.Status200OK, "Lista de vagas retornada com sucesso")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno no servidor")]
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
        [SwaggerOperation(Summary = "Obtém uma vaga específica")]
        [SwaggerResponse(StatusCodes.Status200OK, "Vaga encontrada com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Vaga não encontrada")]
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
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Cria uma nova vaga")]
        [SwaggerResponse(StatusCodes.Status201Created, "Vaga criada com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos")]
        public async Task<IActionResult> CreateVaga([FromBody] Vaga vaga)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Vagas.Add(vaga);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVaga), new { id = vaga.IdVaga }, new
            {
                success = true,
                message = "Vaga criada com sucesso.",
                data = vaga
            });
        }

        // PUT: api/v1/vagas/{id}
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Atualiza uma vaga existente")]
        [SwaggerResponse(StatusCodes.Status200OK, "Vaga atualizada com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Vaga não encontrada")]
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
        [SwaggerOperation(Summary = "Remove uma vaga")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Vaga removida com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Vaga não encontrada")]
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

            return NoContent();
        }
    }
}
