using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Services;
using Asp.Versioning;
using Swashbuckle.AspNetCore.Annotations;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/login")]
    [Tags("Autenticação")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class LoginController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly AppDbContext _context;

        public LoginController(JwtService jwtService, AppDbContext context)
        {
            _jwtService = jwtService;
            _context = context;
        }

        // Classe padronizada de resposta
        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public T? Data { get; set; }

            public static ApiResponse<T> Ok(T? data, string message = "") =>
                new ApiResponse<T> { Success = true, Message = message, Data = data };

            public static ApiResponse<T> Fail(string message) =>
                new ApiResponse<T> { Success = false, Message = message };
        }

        // POST - Autenticação do usuário
        [HttpPost(Name = "Login")]
        [SwaggerOperation(Summary = "Autentica um usuário", Description = "Valida credenciais e retorna um token JWT.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Login realizado com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Usuário ou senha inválidos")]
        public async Task<IActionResult> Autenticar([FromBody] UsuarioLoginInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Senha))
                return BadRequest(ApiResponse<string>.Fail("Dados de login inválidos."));

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == input.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(input.Senha, usuario.Senha))
                return Unauthorized(ApiResponse<string>.Fail("Usuário ou senha inválidos."));

            var token = _jwtService.GenerateToken(usuario.IdUsuario, usuario.Email);

            var data = new
            {
                token,
                usuario = new
                {
                    id = usuario.IdUsuario,
                    email = usuario.Email,
                    nome = usuario.Nome
                }
            };

            return Ok(ApiResponse<object>.Ok(data, "Login realizado com sucesso."));
        }
    }

    // DTO de entrada
    public class UsuarioLoginInput
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
