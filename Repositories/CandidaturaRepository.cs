using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobFitScoreAPI.Repository
{
    public class CandidaturaRepository : ICandidaturaRepository
    {
        private readonly AppDbContext _context;

        public CandidaturaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Candidatura?> GetByIdAsync(int id)
        {
            return await _context.Candidaturas
                .Include(c => c.Usuario)
                .Include(c => c.Vaga)
                .FirstOrDefaultAsync(c => c.IdCandidatura == id);
        }

        public async Task<IEnumerable<Candidatura>> GetAllAsync()
        {
            return await _context.Candidaturas
                .Include(c => c.Usuario)
                .Include(c => c.Vaga)
                .ToListAsync();
        }

        public async Task AddAsync(Candidatura candidatura)
        {
            await _context.Candidaturas.AddAsync(candidatura);
        }

        public void Update(Candidatura candidatura)
        {
            _context.Candidaturas.Update(candidatura);
        }

        public void Delete(Candidatura candidatura)
        {
            _context.Candidaturas.Remove(candidatura);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
