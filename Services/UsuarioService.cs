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

        public async Task<string> LoginAsync(string email, string senha)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            var usuario = usuarios.FirstOrDefault(u => u.Email == email);

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
            var usuarios = await _usuarioRepository.GetAllAsync();
            if (usuarios.Any(u => u.Email == email))
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

            await _usuarioRepository.AddAsync(novoUsuario);
            await _usuarioRepository.SaveChangesAsync();

            return novoUsuario;
        }

        public async Task<IEnumerable<Usuario>> GetAllUsuariosAsync()
        {
            return await _usuarioRepository.GetAllAsync();
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            return await _usuarioRepository.GetByIdAsync(id);
        }

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