using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Services;
using Asp.Versioning;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/login")]
    [ApiVersion("1.0")]
    public class LoginController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly AppDbContext _context;

        public LoginController(JwtService jwtService, AppDbContext context)
        {
            _jwtService = jwtService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Autenticar([FromBody] UsuarioLogin usuarioLogin)
        {
            if (usuarioLogin == null || string.IsNullOrEmpty(usuarioLogin.Email) || string.IsNullOrEmpty(usuarioLogin.Senha))
                return BadRequest(new { mensagem = "Dados de login inválidos." });

            // Usa FirstOrDefaultAsync para evitar exceção caso existam duplicados
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == usuarioLogin.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(usuarioLogin.Senha, usuario.Senha))
                return Unauthorized(new { mensagem = "Usuário ou senha inválidos." });

            var token = _jwtService.GenerateToken(usuario.IdUsuario, usuario.Email);

            return Ok(new { token, email = usuario.Email, nome = usuario.Nome });
        }
    }

    public class UsuarioLogin
    {
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
    }
}
