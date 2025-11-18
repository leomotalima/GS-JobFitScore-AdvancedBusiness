using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByIdAsync(int id);
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task AddAsync(Usuario usuario);
        void Update(Usuario usuario);
        void Delete(Usuario usuario);
        Task<bool> SaveChangesAsync();
    }
}
