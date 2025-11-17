using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Repositories
{
    public interface IHabilidadeRepository
    {
        Task<IEnumerable<Habilidade>> GetAllAsync();
        Task<Habilidade?> GetByIdAsync(int id);
        Task AddAsync(Habilidade habilidade);
        Task UpdateAsync(Habilidade habilidade);
        Task DeleteAsync(Habilidade habilidade);
        Task<bool> ExistsAsync(int id);
    }
}
