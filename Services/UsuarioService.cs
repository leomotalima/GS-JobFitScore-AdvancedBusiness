using JobFitScoreAPI.Models;
using JobFitScoreAPI.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace JobFitScoreAPI.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly JwtService _jwtService;
        private readonly ICryptoService _cryptoService;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            JwtService jwtService,
            ICryptoService cryptoService)
        {
            _usuarioRepository = usuarioRepository;
            _jwtService = jwtService;
            _cryptoService = cryptoService;
        }

        // LOGIN retorna AccessToken + RefreshToken
        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(string email, string senha)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            var usuario = usuarios.FirstOrDefault(u => u.Email == email);

            if (usuario == null)
                throw new Exception("Usuário ou senha inválidos.");

            if (!_cryptoService.VerifyPassword(senha, usuario.Senha))
                throw new Exception("Usuário ou senha inválidos.");

            // Gerar tokens
            string accessToken = _jwtService.GenerateToken(usuario.IdUsuario, usuario.Email, "usuario");
            string refreshToken = _jwtService.GenerateRefreshToken();

            // Salvar refresh token
            usuario.RefreshToken = refreshToken;
            usuario.ExpiraRefreshToken = DateTime.UtcNow.AddDays(7);

            _usuarioRepository.Update(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return (accessToken, refreshToken);
        }

        // RENOVA O TOKEN
        public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            var usuario = usuarios.FirstOrDefault(u => u.RefreshToken == refreshToken);

            if (usuario == null || usuario.ExpiraRefreshToken < DateTime.UtcNow)
                throw new Exception("Refresh Token inválido ou expirado.");

            string newAccessToken = _jwtService.GenerateToken(usuario.IdUsuario, usuario.Email, "usuario");
            string newRefreshToken = _jwtService.GenerateRefreshToken();

            usuario.RefreshToken = newRefreshToken;
            usuario.ExpiraRefreshToken = DateTime.UtcNow.AddDays(7);

            _usuarioRepository.Update(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return (newAccessToken, newRefreshToken);
        }

        // Criar usuário
        public async Task<Usuario> CreateUsuarioAsync(string nome, string email, string senha)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            if (usuarios.Any(u => u.Email == email))
                throw new Exception("E-mail já cadastrado.");

            string senhaHash = _cryptoService.HashPassword(senha);

            var novoUsuario = new Usuario
            {
                Nome = nome,
                Email = email,
                Senha = senhaHash,
            };

            await _usuarioRepository.AddAsync(novoUsuario);
            await _usuarioRepository.SaveChangesAsync();

            return novoUsuario;
        }

        public async Task<IEnumerable<Usuario>> GetAllUsuariosAsync() => 
            await _usuarioRepository.GetAllAsync();

        public async Task<Usuario?> GetUsuarioByIdAsync(int id) => 
            await _usuarioRepository.GetByIdAsync(id);

        public async Task<Usuario?> UpdateUsuarioAsync(int id, Usuario usuario)
        {
            var usuarioExistente = await _usuarioRepository.GetByIdAsync(id);
            if (usuarioExistente == null)
                return null;

            if (!string.IsNullOrWhiteSpace(usuario.Nome))
                usuarioExistente.Nome = usuario.Nome;

            if (!string.IsNullOrWhiteSpace(usuario.Email))
                usuarioExistente.Email = usuario.Email;

            if (!string.IsNullOrWhiteSpace(usuario.Senha))
                usuarioExistente.Senha = _cryptoService.HashPassword(usuario.Senha);

            _usuarioRepository.Update(usuarioExistente);
            await _usuarioRepository.SaveChangesAsync();

            return usuarioExistente;
        }

        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return false;

            _usuarioRepository.Delete(usuario);
            return await _usuarioRepository.SaveChangesAsync();
        }
    }
}
