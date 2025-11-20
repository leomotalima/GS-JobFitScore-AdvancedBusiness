using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobFitScoreAPI.Services
{
    public class VagaService
    {
        private readonly IVagaRepository _vagaRepository;
        private readonly AppDbContext _context;

        public VagaService(IVagaRepository vagaRepository, AppDbContext context)
        {
            _vagaRepository = vagaRepository;
            _context = context;
        }

        public async Task<IEnumerable<Vaga>> GetAllVagasAsync()
        {
            return await _vagaRepository.GetAllAsync();
        }

        public async Task<Vaga?> GetVagaByIdAsync(int id)
        {
            return await _vagaRepository.GetByIdAsync(id);
        }

        public async Task<Vaga> CreateVagaAsync(Vaga vaga)
        {
            if (vaga == null)
                throw new ArgumentNullException(nameof(vaga));

            if (string.IsNullOrWhiteSpace(vaga.Titulo))
                throw new ArgumentException("Título da vaga é obrigatório");

            if (vaga.EmpresaId <= 0)
                throw new ArgumentException("EmpresaId é obrigatório");

            var empresa = await _context.Empresas.FindAsync(vaga.EmpresaId);
            if (empresa == null)
                throw new InvalidOperationException("Empresa não encontrada");

            await _vagaRepository.AddAsync(vaga);
            await _vagaRepository.SaveChangesAsync();

            return vaga;
        }

        public async Task<Vaga?> UpdateVagaAsync(int id, Vaga vaga)
        {
            if (vaga == null)
                throw new ArgumentNullException(nameof(vaga));

            var vagaExistente = await _vagaRepository.GetByIdAsync(id);
            if (vagaExistente == null)
                return null;

            if (!string.IsNullOrWhiteSpace(vaga.Titulo))
                vagaExistente.Titulo = vaga.Titulo;

            _vagaRepository.Update(vagaExistente);
            await _vagaRepository.SaveChangesAsync();

            return vagaExistente;
        }

        public async Task<bool> DeleteVagaAsync(int id)
        {
            var vaga = await _vagaRepository.GetByIdAsync(id);
            if (vaga == null)
                return false;

            _vagaRepository.Delete(vaga);
            return await _vagaRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Vaga>> GetVagasByEmpresaAsync(int empresaId)
        {
            var vagas = await _vagaRepository.GetAllAsync();
            return vagas.Where(v => v.EmpresaId == empresaId);
        }

        public async Task<IEnumerable<Vaga>> SearchVagasAsync(string? titulo)
        {
            var vagas = await _vagaRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(titulo))
                vagas = vagas.Where(v => v.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase));

            return vagas;
        }
    }
}
