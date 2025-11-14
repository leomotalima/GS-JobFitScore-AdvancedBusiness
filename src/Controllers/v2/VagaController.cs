using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;

namespace JobFitScoreAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/vaga")]
    [ApiVersion("2.0")]
    public class VagaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VagaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("filtrar")]
        public async Task<IActionResult> Filtrar(decimal? salarioMin, decimal? salarioMax, string? nivel)
        {
            var query = _context.Vagas.AsQueryable();

            if (salarioMin.HasValue)
                query = query.Where(v => v.Salario >= salarioMin.Value);

            if (salarioMax.HasValue)
                query = query.Where(v => v.Salario <= salarioMax.Value);

            if (!string.IsNullOrWhiteSpace(nivel))
                query = query.Where(v =>
                    v.NivelExperiencia != null &&
                    v.NivelExperiencia.Contains(nivel, StringComparison.OrdinalIgnoreCase)
                );

            var result = await query
                .Select(v => new
                {
                    v.IdVaga,
                    v.Titulo,
                    v.Salario,
                    v.NivelExperiencia,
                    v.Localizacao
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
