using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobFitScoreAPI.Repository
{
    public class VagaRepository : IVagaRepository
    {
        private readonly AppDbContext _context;

        public VagaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Vaga?> GetByIdAsync(int id)
        {
            return await _context.Vagas.FindAsync(id);
        }

        public async Task<IEnumerable<Vaga>> GetAllAsync()
        {
            return await _context.Vagas.ToListAsync();
        }

        public async Task AddAsync(Vaga vaga)
        {
            await _context.Vagas.AddAsync(vaga);
        }

        public void Update(Vaga vaga)
        {
            _context.Vagas.Update(vaga);
        }

        public void Delete(Vaga vaga)
        {
            _context.Vagas.Remove(vaga);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
