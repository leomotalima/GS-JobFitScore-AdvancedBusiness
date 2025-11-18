using Microsoft.AspNetCore.Mvc;   
using Asp.Versioning;              
using Microsoft.EntityFrameworkCore; 
using JobFitScoreAPI.Data;         
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/habilidade")]
    [Asp.Versioning.ApiVersion("2.0")]
    public class HabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HabilidadeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var habilidades = await _context.Habilidades
                .Select(h => new
                {
                    h.IdHabilidade,
                    h.Nome
                })
                .ToListAsync();

            return Ok(habilidades);
        }
    }
}
