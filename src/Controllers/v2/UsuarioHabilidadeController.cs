using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;

namespace JobFitScoreAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/usuariohabilidade")]
    [ApiVersion("2.0")]
    public class UsuarioHabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioHabilidadeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetByUsuario(int usuarioId)
        {
            var result = await _context.UsuarioHabilidades
                .Where(uh => uh.UsuarioId == usuarioId)
                .Select(uh => new
                {
                    uh.IdUsuarioHabilidade,
                    uh.UsuarioId,
                    Habilidade = uh.Habilidade!.Nome
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
