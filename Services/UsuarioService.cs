using JobFitScoreAPI.Data;
using JobFitScoreAPI.Services;
using JobFitScoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JobFitScoreAPI.Services
{
    public class UsuarioService 
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        private readonly ICryptoService _cryptoService;

        public UsuarioService(
            AppDbContext context, 
            JwtService jwtService, 
            ICryptoService cryptoService)
        {
            _context = context;
            _jwtService = jwtService;
            _cryptoService = cryptoService;
        }

        public async Task<string> LoginAsync(string email, string senha)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null)
                throw new Exception("Usuário ou senha inválidos.");

            if (!_cryptoService.VerifyPassword(senha, usuario.Senha))
            {
                throw new Exception("Usuário ou senha inválidos."); 
            }
            
            return _jwtService.GenerateToken(usuario.IdUsuario, usuario.Email);
        }

        public async Task<Usuario> CreateUsuarioAsync(string nome, string email, string senha) 
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            {
                throw new Exception("E-mail já cadastrado.");
            }
            
            string senhaHash = _cryptoService.HashPassword(senha); 
            
            var novoUsuario = new Usuario 
            {
                Nome = nome,
                Email = email,
                Senha = senhaHash,
            };

            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();
            
            return novoUsuario;
        }
    }
}