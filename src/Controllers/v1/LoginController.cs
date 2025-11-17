using Microsoft.AspNetCore.Mvc;
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

        public LoginController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost]
        public IActionResult Autenticar([FromBody] UsuarioLogin usuarioLogin)
        {
            if (usuarioLogin == null || string.IsNullOrEmpty(usuarioLogin.Email) || string.IsNullOrEmpty(usuarioLogin.Senha))
            {
                return BadRequest(new { mensagem = "Dados de login inválidos." });
            }

            
            bool usuarioValido = (usuarioLogin.Email == "teste@jobfit.com" && usuarioLogin.Senha == "123456");
            if (!usuarioValido)
                return Unauthorized(new { mensagem = "Usuário ou senha inválidos." });

            // ID fictício para teste (em produção, pegue do banco)
            int usuarioId = 1;

            // Gerar token usando o ID fictício e o email do usuário
            var token = _jwtService.GenerateToken(usuarioId, usuarioLogin.Email);

            return Ok(new { token });
        }
    }
}
