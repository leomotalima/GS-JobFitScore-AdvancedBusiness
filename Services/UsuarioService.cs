using JobFitScoreAPI.Data;
using JobFitScoreAPI.Services;
using JobFitScoreAPI.Models;
using System;
using System.Linq;

namespace JobFitScoreAPI.Services
{
    public class UsuarioService
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public UsuarioService(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public string Login(string email, string senha)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);
            if (usuario == null)
                throw new Exception("Usuário ou senha inválidos.");

            return _jwtService.GenerateToken(usuario.Email);
        }
    }
}
