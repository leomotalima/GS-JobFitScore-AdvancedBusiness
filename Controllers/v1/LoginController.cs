using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Services;
using Asp.Versioning;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

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

    [HttpPost(Name = "Login")] 
    [SwaggerOperation(Summary = "Autentica um usuário", Description = "Valida credenciais e retorna um token JWT e Refresh Token.")] 
    public async Task<IActionResult> Autenticar([FromBody] UsuarioLoginInput input) 
    { 
        if (input == null || string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Senha)) 
            return BadRequest(ApiResponse<string>.Fail("Dados de login inválidos.")); 

        // Busca usuário ignorando maiúsculas/minúsculas 
        var usuario = await _context.Usuarios 
            .FirstOrDefaultAsync(u => u.Email.ToLower() == input.Email.ToLower()); 

        // DEBUG TEMPORÁRIO: Imprime os valores antes da verificação
        if (usuario != null)
        {
            Console.WriteLine("\n--- DEBUG LOGIN ---");
            Console.WriteLine($"E-mail: {usuario.Email}");
            Console.WriteLine($"Senha Digitada (Trim): '{input.Senha.Trim()}'");
            Console.WriteLine($"Hash LIDO DO BD (Trim): '{usuario.Senha.Trim()}'");
            Console.WriteLine("-------------------\n");
        }
        // FIM DO DEBUG TEMPORÁRIO

        // Verifica usuário e senha com Trim() para evitar espaços extras 
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(input.Senha.Trim(), usuario.Senha.Trim())) 
            return Unauthorized(ApiResponse<string>.Fail("Usuário ou senha inválidos.")); 

        // Gera tokens 
        var accessToken = _jwtService.GenerateToken(usuario.IdUsuario, usuario.Email); 
        var refreshToken = _jwtService.GenerateRefreshToken(); 

        // Salva Refresh Token no banco 
        usuario.RefreshToken = refreshToken; 
        usuario.ExpiraRefreshToken = DateTime.UtcNow.AddDays(7); 
        await _context.SaveChangesAsync(); 

        var data = new 
        { 
            access_token = accessToken, 
            refresh_token = refreshToken, 
            usuario = new 
            { 
                id = usuario.IdUsuario, 
                email = usuario.Email, 
                nome = usuario.Nome 
            } 
        }; 

        return Ok(ApiResponse<object>.Ok(data, "Login realizado com sucesso.")); 
    } 

    [HttpPost("refresh", Name = "RefreshToken")] 
    [SwaggerOperation( 
        Summary = "Renova o token JWT", 
        Description = "Gera novo access token usando um refresh token válido." 
    )] 
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenInput input) 
    { 
        if (string.IsNullOrWhiteSpace(input.RefreshToken)) 
            return BadRequest(ApiResponse<string>.Fail("Refresh Token inválido.")); 

        var usuario = await _context.Usuarios 
            .FirstOrDefaultAsync(u => u.RefreshToken == input.RefreshToken); 

        if (usuario == null || usuario.ExpiraRefreshToken < DateTime.UtcNow) 
            return Unauthorized(ApiResponse<string>.Fail("Refresh Token inválido ou expirado.")); 

        var newAccessToken = _jwtService.GenerateToken(usuario.IdUsuario, usuario.Email); 
        var newRefreshToken = _jwtService.GenerateRefreshToken(); 

        usuario.RefreshToken = newRefreshToken; 
        usuario.ExpiraRefreshToken = DateTime.UtcNow.AddDays(7); 
        await _context.SaveChangesAsync(); 

        var data = new 
        { 
            access_token = newAccessToken, 
            refresh_token = newRefreshToken 
        }; 

        return Ok(ApiResponse<object>.Ok(data, "Token renovado com sucesso.")); 
    } 
} 

// DTOs 
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