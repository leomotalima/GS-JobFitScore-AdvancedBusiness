using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JobFitScoreAPI.Repositories
{
    public class HabilidadeRepository : IHabilidadeRepository
    {
        private readonly AppDbContext _context;

        public HabilidadeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Habilidade>> GetAllAsync()
        {
            return await _context.Habilidades.ToListAsync();
        }

        public async Task<Habilidade?> GetByIdAsync(int id)
        {
            return await _context.Habilidades
                                 .FirstOrDefaultAsync(h => h.IdHabilidade == id);
        }

        public async Task AddAsync(Habilidade habilidade)
        {
            await _context.Habilidades.AddAsync(habilidade);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Habilidade habilidade)
        {
            _context.Habilidades.Update(habilidade);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Habilidade habilidade)
        {
            _context.Habilidades.Remove(habilidade);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Habilidades.AnyAsync(h => h.IdHabilidade == id);
        }
    }
}
