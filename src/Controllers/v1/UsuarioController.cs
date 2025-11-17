using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/usuario")]
    [Asp.Versioning.ApiVersion("1.0")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
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
                .OrderBy(u => u.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new
                {
                    u.IdUsuario,
                    u.Nome,
                    Email = u.Email ?? string.Empty
                })
                .ToListAsync();

            var result = new
            {
                totalItems = total,
                currentPage = page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize),
                data = usuarios
            };

            return Ok(result);
        }

        // GET: api/v1/usuario/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            var result = new
            {
                usuario.IdUsuario,
                usuario.Nome,
                Email = usuario.Email ?? string.Empty
            };

            return Ok(result);
        }

        
        // POST: api/v1/usuario
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Usuario usuario)
        {
            if (usuario == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var result = new
            {
                usuario.IdUsuario,
                usuario.Nome,
                Email = usuario.Email ?? string.Empty
            };

            return CreatedAtAction(nameof(GetById), new { id = usuario.IdUsuario, version = "1.0" }, result);
        }

        
        // PUT: api/v1/usuario/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Usuario updated)
        {
            if (updated == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            usuario.Nome = updated.Nome ?? usuario.Nome;
            usuario.Email = updated.Email ?? usuario.Email;
            usuario.Senha = updated.Senha ?? usuario.Senha;
            usuario.RefreshToken = updated.RefreshToken ?? usuario.RefreshToken;
            usuario.ExpiraRefreshToken = updated.ExpiraRefreshToken ?? usuario.ExpiraRefreshToken;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        // DELETE: api/v1/usuario/{id}
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

        // GET: api/v1/usuario/search?nome=abc
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
                    Email = u.Email ?? string.Empty
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
