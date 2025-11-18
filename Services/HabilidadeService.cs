using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JobFitScoreAPI.Services
{
    public class HabilidadeService
    {
        private readonly IHabilidadeRepository _habilidadeRepository;
        private readonly AppDbContext _context;

        public HabilidadeService(IHabilidadeRepository habilidadeRepository, AppDbContext context)
        {
            _habilidadeRepository = habilidadeRepository;
            _context = context;
        }

        public async Task<IEnumerable<Habilidade>> GetAllHabilidadesAsync()
        {
            return await _habilidadeRepository.GetAllAsync();
        }

        public async Task<Habilidade?> GetHabilidadeByIdAsync(int id)
        {
            return await _habilidadeRepository.GetByIdAsync(id);
        }

        public async Task<Habilidade> CreateHabilidadeAsync(Habilidade habilidade)
        {
            if (habilidade == null)
                throw new ArgumentNullException(nameof(habilidade));

            if (string.IsNullOrWhiteSpace(habilidade.NomeHabilidade))
                throw new ArgumentException("Nome da habilidade é obrigatório");

            var habilidadeExistente = (await _habilidadeRepository.GetAllAsync())
                .FirstOrDefault(h => h.NomeHabilidade.Equals(habilidade.NomeHabilidade, StringComparison.OrdinalIgnoreCase));

            if (habilidadeExistente != null)
                throw new InvalidOperationException("Habilidade já cadastrada com este nome");

            await _habilidadeRepository.AddAsync(habilidade);
            return habilidade;
        }

        public async Task<Habilidade?> UpdateHabilidadeAsync(int id, Habilidade habilidade)
        {
            if (habilidade == null)
                throw new ArgumentNullException(nameof(habilidade));

            var habilidadeExistente = await _habilidadeRepository.GetByIdAsync(id);
            if (habilidadeExistente == null)
                return null;

            if (!string.IsNullOrWhiteSpace(habilidade.NomeHabilidade))
                habilidadeExistente.NomeHabilidade = habilidade.NomeHabilidade;

            if (!string.IsNullOrWhiteSpace(habilidade.Categoria))
                habilidadeExistente.Categoria = habilidade.Categoria;

            if (!string.IsNullOrWhiteSpace(habilidade.Descricao))
                habilidadeExistente.Descricao = habilidade.Descricao;

            await _habilidadeRepository.UpdateAsync(habilidadeExistente);
            return habilidadeExistente;
        }

        public async Task<bool> DeleteHabilidadeAsync(int id)
        {
            if (!await _habilidadeRepository.ExistsAsync(id))
                return false;

            var habilidade = await _habilidadeRepository.GetByIdAsync(id);
            if (habilidade != null)
            {
                await _habilidadeRepository.DeleteAsync(habilidade);
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Habilidade>> SearchHabilidadesAsync(string? nome, string? categoria)
        {
            var habilidades = await _habilidadeRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(nome))
                habilidades = habilidades.Where(h => h.NomeHabilidade.Contains(nome, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(categoria))
                habilidades = habilidades.Where(h => h.Categoria != null && 
                    h.Categoria.Contains(categoria, StringComparison.OrdinalIgnoreCase));

            return habilidades;
        }

        public async Task<IEnumerable<Habilidade>> GetHabilidadesByCategoriaAsync(string categoria)
        {
            var habilidades = await _habilidadeRepository.GetAllAsync();
            return habilidades.Where(h => h.Categoria != null && 
                h.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));
        }
    }
}
