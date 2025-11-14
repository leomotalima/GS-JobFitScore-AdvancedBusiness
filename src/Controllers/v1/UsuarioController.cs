using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Dtos.Usuario;
using BCrypt.Net;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public UsuarioController(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
        }

        // ============================================================
        // GET: api/v1/usuario?page=1&pageSize=5
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 5)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { mensagem = "Parâmetros de paginação inválidos." });

            var total = await _context.Usuarios.CountAsync();

            var usuarios = await _context.Usuarios
                .OrderBy(u => u.IdUsuario)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Select(u => new UsuarioOutput
                {
                    IdUsuario = u.IdUsuario,
                    Nome = u.Nome,
                    Email = u.Email,
                    Habilidades = u.Habilidades
                })
                .ToListAsync();

            var result = new
            {
                totalItems = total,
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize),
                data = usuarios,
                links = new List<object>
                {
                    new { rel = "self", href = GetPageUrl(page, pageSize), method = "GET" },
                    new { rel = "next", href = GetPageUrl(page + 1, pageSize), method = "GET" },
                    new { rel = "previous", href = GetPageUrl(page - 1, pageSize), method = "GET" }
                }
            };

            return Ok(result);
        }

        // ============================================================
        // GET: api/v1/usuario/{id}
        // ============================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            var result = new UserOutputForDetails(usuario.IdUsuario, usuario.Nome, usuario.Email, usuario.Habilidades, GenerateLinks(id));
            
            return Ok(result);
        }

        // ============================================================
        // POST: api/v1/usuario
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioInput input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = new Usuario
            {
                Nome = input.Nome,
                Email = input.Email,
                Habilidades = input.Habilidades,
                Senha = BCrypt.Net.BCrypt.HashPassword(input.Senha)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var url = GetByIdUrl(usuario.IdUsuario);

            var result = new UsuarioOutput
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Habilidades = usuario.Habilidades
            };

            return Created(url, new { result, links = GenerateLinks(usuario.IdUsuario) });
        }

        // ============================================================
        // PUT: api/v1/usuario/{id}
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioUpdateInput input)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            usuario.Nome = input.Nome;
            usuario.Email = input.Email;
            usuario.Habilidades = input.Habilidades;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // DELETE: api/v1/usuario/{id}
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // MÉTODOS AUXILIARES - URLs e HATEOAS
        // ============================================================
        private IEnumerable<object> GenerateLinks(int id) =>
            new List<object>
            {
                new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                new { rel = "update", href = GetByIdUrl(id), method = "PUT" },
                new { rel = "delete", href = GetByIdUrl(id), method = "DELETE" },
                new { rel = "all", href = GetPageUrl(1, 5), method = "GET" }
            };

        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetById), "Usuario", new { id }) ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetAll), "Usuario", new { page, pageSize }) ?? string.Empty;
    }

    // DTO opcional para deixar o GET/{id} mais limpo
    public record UserOutputForDetails(int IdUsuario, string Nome, string Email, string? Habilidades, IEnumerable<object> Links);
}
