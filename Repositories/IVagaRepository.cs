using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Repository.Interfaces
{
    public interface IVagaRepository
    {
        Task<Vaga?> GetByIdAsync(int id);
        Task<IEnumerable<Vaga>> GetAllAsync();
        Task AddAsync(Vaga vaga);
        void Update(Vaga vaga);
        void Delete(Vaga vaga);
        Task<bool> SaveChangesAsync();
    }
}
