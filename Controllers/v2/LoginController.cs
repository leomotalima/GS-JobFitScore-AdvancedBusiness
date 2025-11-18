using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Services;
using Asp.Versioning;
using Swashbuckle.AspNetCore.Annotations;

namespace JobFitScoreAPI.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/login")]
    [Tags("Autenticação v2")]
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

        // Classe de resposta padronizada
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

        // POST - Autenticação automática (usuário ou empresa)
        [HttpPost(Name = "LoginV2")]
        [SwaggerOperation(
            Summary = "Autentica usuário ou empresa",
            Description = "Valida credenciais verificando automaticamente nas tabelas de usuários e empresas. Retorna um token JWT com o tipo identificado."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Login realizado com sucesso", typeof(ApiResponse<object>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Email ou senha inválidos")]
        public async Task<IActionResult> Autenticar([FromBody] LoginInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Senha))
                return BadRequest(ApiResponse<string>.Fail("Email e senha são obrigatórios."));

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == input.Email);

            if (usuario != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(input.Senha, usuario.Senha))
                    return Unauthorized(ApiResponse<string>.Fail("Email ou senha inválidos."));

                var token = _jwtService.GenerateToken(usuario.IdUsuario, usuario.Email, "usuario");

                var data = new
                {
                    token,
                    userType = "usuario",
                    user = new
                    {
                        id = usuario.IdUsuario,
                        email = usuario.Email,
                        nome = usuario.Nome
                    }
                };

                return Ok(ApiResponse<object>.Ok(data, "Login realizado com sucesso como usuário."));
            }

            var empresa = await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Email == input.Email);

            if (empresa != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(input.Senha, empresa.Senha))
                    return Unauthorized(ApiResponse<string>.Fail("Email ou senha inválidos."));

                var token = _jwtService.GenerateToken(empresa.IdEmpresa, empresa.Email, "empresa");

                var data = new
                {
                    token,
                    userType = "empresa",
                    empresa = new
                    {
                        id = empresa.IdEmpresa,
                        email = empresa.Email,
                        nome = empresa.Nome,
                        cnpj = empresa.Cnpj
                    }
                };

                return Ok(ApiResponse<object>.Ok(data, "Login realizado com sucesso como empresa."));
            }

            return Unauthorized(ApiResponse<string>.Fail("Email ou senha inválidos."));
        }

        // POST - Autenticação por tipo específico
        [HttpPost("tipo/{tipo}", Name = "LoginV2ComTipo")]
        [SwaggerOperation(
            Summary = "Autentica especificando o tipo",
            Description = "Permite especificar explicitamente se está autenticando como 'usuario' ou 'empresa'."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Login realizado com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Tipo ou dados inválidos")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Email ou senha inválidos")]
        public async Task<IActionResult> AutenticarPorTipo(
            [FromRoute] string tipo,
            [FromBody] LoginInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Senha))
                return BadRequest(ApiResponse<string>.Fail("Email e senha são obrigatórios."));

            tipo = tipo.ToLower();

            if (tipo != "usuario" && tipo != "empresa")
                return BadRequest(ApiResponse<string>.Fail("Tipo deve ser 'usuario' ou 'empresa'."));

            if (tipo == "usuario")
            {
                var usuario = await _context.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == input.Email);

                if (usuario == null || !BCrypt.Net.BCrypt.Verify(input.Senha, usuario.Senha))
                    return Unauthorized(ApiResponse<string>.Fail("Email ou senha inválidos."));

                var token = _jwtService.GenerateToken(usuario.IdUsuario, usuario.Email, "usuario");

                var data = new
                {
                    token,
                    userType = "usuario",
                    user = new
                    {
                        id = usuario.IdUsuario,
                        email = usuario.Email,
                        nome = usuario.Nome
                    }
                };

                return Ok(ApiResponse<object>.Ok(data, "Login realizado com sucesso como usuário."));
            }
            else
            {
                var empresa = await _context.Empresas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Email == input.Email);

                if (empresa == null || !BCrypt.Net.BCrypt.Verify(input.Senha, empresa.Senha))
                    return Unauthorized(ApiResponse<string>.Fail("Email ou senha inválidos."));

                var token = _jwtService.GenerateToken(empresa.IdEmpresa, empresa.Email, "empresa");

                var data = new
                {
                    token,
                    userType = "empresa",
                    empresa = new
                    {
                        id = empresa.IdEmpresa,
                        email = empresa.Email,
                        nome = empresa.Nome,
                        cnpj = empresa.Cnpj
                    }
                };

                return Ok(ApiResponse<object>.Ok(data, "Login realizado com sucesso como empresa."));
            }
        }
    }

    public class LoginInput
    {
        [SwaggerSchema(Description = "Email do usuário ou empresa")]
        public string Email { get; set; } = string.Empty;

        [SwaggerSchema(Description = "Senha")]
        public string Senha { get; set; } = string.Empty;
    }
}
