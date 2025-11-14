using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/login")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public LoginController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // DTOs
        public class LoginDto
        {
            public string Email { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }

        public class LoginResponseDto
        {
            public string Token { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        // POST: api/v1/login
        [HttpPost]
        public IActionResult Login([FromBody] LoginDto login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Senha))
                return BadRequest(new { message = "Dados inválidos." });

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == login.Email);

            // Mensagem genérica para evitar "user enumeration"
            var mensagemErro = new { message = "Usuário ou senha inválidos." };

            if (usuario == null)
                return Unauthorized(mensagemErro);

            bool senhaOk = BCrypt.Net.BCrypt.Verify(login.Senha, usuario.Senha);

            if (!senhaOk)
                return Unauthorized(mensagemErro);

            var token = _jwtService.GenerateToken(usuario.IdUsuario, usuario.Email);

            var response = new LoginResponseDto
            {
                Token = token,
                Email = usuario.Email
            };

            return Ok(response);
        }
    }
}
