using Microsoft.AspNetCore.Mvc;   
using Asp.Versioning;              
using Microsoft.EntityFrameworkCore; 
using JobFitScoreAPI.Data;         
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/vagas")]
    [ApiVersion("2.0")]
    public class VagaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VagaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vagas = await _context.Vagas
                .Include(v => v.Empresa)
                .Select(v => new 
                {
                    v.IdVaga,
                    v.Titulo,
                    Empresa = v.Empresa != null ? v.Empresa.Nome : null
                })
                .ToListAsync();

            return Ok(vagas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vaga = await _context.Vagas
                .Include(v => v.Empresa)
                .Where(v => v.IdVaga == id)
                .Select(v => new 
                {
                    v.IdVaga,
                    v.Titulo,
                    Empresa = v.Empresa != null ? v.Empresa.Nome : null
                })
                .FirstOrDefaultAsync();

            if (vaga == null)
                return NotFound(new { success = false, message = "Vaga não encontrada." });

            return Ok(vaga);
        }
    }
}
