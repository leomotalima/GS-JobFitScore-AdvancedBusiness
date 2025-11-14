using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v1/login")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public LoginController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public class LoginDto
        {
            public string Email { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto login)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == login.Email && u.Senha == login.Senha);

            if (usuario == null)
                return NotFound(new { message = "Usuário ou senha inválidos." });

            var token = _jwtService.GenerateToken(usuario.IdUsuario, usuario.Email);

            return Ok(new
            {
                token,
                usuario = usuario.Email
            });
        }
    }
}
