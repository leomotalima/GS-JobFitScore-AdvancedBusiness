using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Services;
using Asp.Versioning;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace JobFitScoreAPI.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
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

        [AllowAnonymous]
        [HttpPost(Name = "Login")]
        [SwaggerOperation(Summary = "Autentica um usuário ou empresa", Description = "Valida credenciais e retorna um token JWT e Refresh Token.")]
        public async Task<IActionResult> Autenticar([FromBody] UsuarioLoginInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Senha))
                return BadRequest(ApiResponse<string>.Fail("Dados de login inválidos."));

            var email = input.Email.Trim().ToLower();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == email);

            var empresa = usuario == null ? await _context.Empresas.FirstOrDefaultAsync(e => e.Email.ToLower() == email) : null;

            if (usuario == null && empresa == null)
                return Unauthorized(ApiResponse<string>.Fail("Usuário ou senha inválidos."));

            if (usuario != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(input.Senha, usuario.Senha))
                    return Unauthorized(ApiResponse<string>.Fail("Usuário ou senha inválidos."));

                var accessToken = _jwtService.GenerateToken(usuario.IdUsuario, usuario.Email, "usuario");
                var refreshToken = _jwtService.GenerateRefreshToken();

                usuario.RefreshToken = refreshToken;
                usuario.ExpiraRefreshToken = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                var data = new
                {
                    access_token = accessToken,
                    refresh_token = refreshToken,
                    tipo = "usuario",
                    usuario = new { id = usuario.IdUsuario, email = usuario.Email, nome = usuario.Nome }
                };

                return Ok(ApiResponse<object>.Ok(data, "Login realizado com sucesso."));
            }

            if (empresa != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(input.Senha, empresa.Senha))
                    return Unauthorized(ApiResponse<string>.Fail("Usuário ou senha inválidos."));

                var accessTokenE = _jwtService.GenerateToken(empresa.IdEmpresa, empresa.Email, "empresa");
                var refreshTokenE = _jwtService.GenerateRefreshToken();

                empresa.RefreshToken = refreshTokenE;
                empresa.ExpiraRefreshToken = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                var dataEmpresa = new
                {
                    access_token = accessTokenE,
                    refresh_token = refreshTokenE,
                    tipo = "empresa",
                    empresa = new { id = empresa.IdEmpresa, email = empresa.Email, nome = empresa.Nome }
                };

                return Ok(ApiResponse<object>.Ok(dataEmpresa, "Login realizado com sucesso."));
            }

            return Unauthorized(ApiResponse<string>.Fail("Usuário ou senha inválidos."));
        }

        [AllowAnonymous]
        [HttpPost("refresh", Name = "RefreshToken")]
        [SwaggerOperation(Summary = "Renova o token JWT", Description = "Gera novo access token usando um refresh token válido.")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenInput input)
        {
            if (string.IsNullOrWhiteSpace(input.RefreshToken))
                return BadRequest(ApiResponse<string>.Fail("Refresh Token inválido."));

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.RefreshToken == input.RefreshToken);

            if (usuario != null)
            {
                if (usuario.ExpiraRefreshToken == null || usuario.ExpiraRefreshToken < DateTime.UtcNow)
                    return Unauthorized(ApiResponse<string>.Fail("Refresh Token expirado."));

                var newAccessToken = _jwtService.GenerateToken(usuario.IdUsuario, usuario.Email, "usuario");
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                usuario.RefreshToken = newRefreshToken;
                usuario.ExpiraRefreshToken = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                var data = new { access_token = newAccessToken, refresh_token = newRefreshToken, tipo = "usuario" };
                return Ok(ApiResponse<object>.Ok(data, "Token renovado com sucesso."));
            }

            var empresa = await _context.Empresas.FirstOrDefaultAsync(e => e.RefreshToken == input.RefreshToken);

            if (empresa != null)
            {
                if (empresa.ExpiraRefreshToken == null || empresa.ExpiraRefreshToken < DateTime.UtcNow)
                    return Unauthorized(ApiResponse<string>.Fail("Refresh Token expirado."));

                var newAccessToken = _jwtService.GenerateToken(empresa.IdEmpresa, empresa.Email, "empresa");
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                empresa.RefreshToken = newRefreshToken;
                empresa.ExpiraRefreshToken = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                var data = new { access_token = newAccessToken, refresh_token = newRefreshToken, tipo = "empresa" };
                return Ok(ApiResponse<object>.Ok(data, "Token renovado com sucesso."));
            }

            return Unauthorized(ApiResponse<string>.Fail("Refresh Token inválido."));
        }
    }

    public class UsuarioLoginInput
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class RefreshTokenInput
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}