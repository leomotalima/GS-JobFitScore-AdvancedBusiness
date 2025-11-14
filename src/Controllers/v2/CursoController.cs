using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;

namespace JobFitScoreAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/curso")]
    [ApiVersion("2.0")]
    public class CursoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CursoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("filtrar")]
        public async Task<IActionResult> Filtrar(int? cargaMin, int? cargaMax)
        {
            var query = _context.Cursos.AsQueryable();

            if (cargaMin.HasValue)
                query = query.Where(c => c.CargaHoraria >= cargaMin.Value);

            if (cargaMax.HasValue)
                query = query.Where(c => c.CargaHoraria <= cargaMax.Value);

            var result = await query
                .Select(c => new
                {
                    c.IdCurso,
                    c.Nome,
                    c.CargaHoraria
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
