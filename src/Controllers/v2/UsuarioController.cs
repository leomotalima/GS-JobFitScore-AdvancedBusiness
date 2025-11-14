using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;

namespace JobFitScoreAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/usuario")]
    [ApiVersion("2.0")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? nome, string? habilidade)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(u => u.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(habilidade))
                query = query.Where(u => u.Habilidades != null && u.Habilidades.Contains(habilidade));

            var result = await query
                .Select(u => new
                {
                    u.IdUsuario,
                    u.Nome,
                    u.Email,
                    u.Habilidades
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
