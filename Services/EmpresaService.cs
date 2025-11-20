using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JobFitScoreAPI.Services
{
    public class EmpresaService
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly JwtService _jwtService;
        private readonly AppDbContext _context;

        public EmpresaService(IEmpresaRepository empresaRepository, JwtService jwtService, AppDbContext context)
        {
            _empresaRepository = empresaRepository;
            _jwtService = jwtService;
            _context = context;
        }

        public async Task<IEnumerable<Empresa>> GetAllEmpresasAsync()
        {
            return await _empresaRepository.GetAllAsync();
        }

        public async Task<Empresa?> GetEmpresaByIdAsync(int id)
        {
            return await _empresaRepository.GetByIdAsync(id);
        }

        public async Task<Empresa> CreateEmpresaAsync(Empresa empresa)
        {
            if (empresa == null)
                throw new ArgumentNullException(nameof(empresa));

            if (string.IsNullOrWhiteSpace(empresa.NomeEmpresa))
                throw new ArgumentException("Nome da empresa é obrigatório");

            if (string.IsNullOrWhiteSpace(empresa.Email))
                throw new ArgumentException("Email é obrigatório");

            if (string.IsNullOrWhiteSpace(empresa.Senha))
                throw new ArgumentException("Senha é obrigatória");

            if (string.IsNullOrWhiteSpace(empresa.Cnpj))
                throw new ArgumentException("CNPJ é obrigatório");

            var empresaExistente = (await _empresaRepository.GetAllAsync())
                .FirstOrDefault(e => e.Email == empresa.Email || e.Cnpj == empresa.Cnpj);

            if (empresaExistente != null)
                throw new InvalidOperationException("Empresa já cadastrada com este email ou CNPJ");

            await _empresaRepository.AddAsync(empresa);
            return empresa;
        }

        public async Task<Empresa?> UpdateEmpresaAsync(int id, Empresa empresa)
        {
            if (empresa == null)
                throw new ArgumentNullException(nameof(empresa));

            var empresaExistente = await _empresaRepository.GetByIdAsync(id);
            if (empresaExistente == null)
                return null;

            if (!string.IsNullOrWhiteSpace(empresa.NomeEmpresa))
                empresaExistente.NomeEmpresa = empresa.NomeEmpresa;

            if (!string.IsNullOrWhiteSpace(empresa.Email))
                empresaExistente.Email = empresa.Email;

            if (!string.IsNullOrWhiteSpace(empresa.Senha))
                empresaExistente.Senha = empresa.Senha;

            if (!string.IsNullOrWhiteSpace(empresa.Cnpj))
                empresaExistente.Cnpj = empresa.Cnpj;

            await _empresaRepository.UpdateAsync(empresaExistente);
            return empresaExistente;
        }

        public async Task<bool> DeleteEmpresaAsync(int id)
        {
            if (!await _empresaRepository.ExistsAsync(id))
                return false;

            var empresa = await _empresaRepository.GetByIdAsync(id);
            if (empresa != null)
            {
                await _empresaRepository.DeleteAsync(empresa);
                return true;
            }

            return false;
        }

        
        public async Task<string> LoginEmpresaAsync(string email, string senha)
        {
            var empresas = await _empresaRepository.GetAllAsync();
            var empresa = empresas.FirstOrDefault(e => e.Email == email && e.Senha == senha);

            if (empresa == null)
                throw new UnauthorizedAccessException("Email ou senha inválidos");

            
            return _jwtService.GenerateToken(empresa.IdEmpresa, empresa.Email, "empresa");
        }

        public async Task<IEnumerable<Empresa>> SearchEmpresasAsync(string? nome)
        {
            var empresas = await _empresaRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(nome))
                empresas = empresas.Where(e =>
                    e.NomeEmpresa.Contains(nome, StringComparison.OrdinalIgnoreCase));

            return empresas;
        }
    }
}
