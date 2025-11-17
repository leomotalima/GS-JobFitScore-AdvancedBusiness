using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Repositories
{
    public interface IEmpresaRepository
    {
        Task<IEnumerable<Empresa>> GetAllAsync();
        Task<Empresa?> GetByIdAsync(int id);
        Task AddAsync(Empresa empresa);
        Task UpdateAsync(Empresa empresa);
        Task DeleteAsync(Empresa empresa);
        Task<bool> ExistsAsync(int id);
    }
}
