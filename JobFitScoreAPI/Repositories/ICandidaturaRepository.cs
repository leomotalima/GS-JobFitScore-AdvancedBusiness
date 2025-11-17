using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Repository.Interfaces
{
    public interface ICandidaturaRepository
    {
        Task<Candidatura?> GetByIdAsync(int id);
        Task<IEnumerable<Candidatura>> GetAllAsync();
        Task AddAsync(Candidatura candidatura);
        void Update(Candidatura candidatura);
        void Delete(Candidatura candidatura);
        Task<bool> SaveChangesAsync();
    }
}
