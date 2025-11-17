using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;

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
        public async Task<IActionResult> Search(string? nome)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(u => u.Nome.Contains(nome));

            var result = await query
                .Select(u => new
                {
                    u.IdUsuario,
                    u.Nome,
                    u.Email,
                    Habilidades = _context.UsuarioHabilidades
                        .Where(uh => uh.UsuarioId == u.IdUsuario && uh.Habilidade != null)
                        .Select(uh => uh.Habilidade!.Nome)
                        .ToList()
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
