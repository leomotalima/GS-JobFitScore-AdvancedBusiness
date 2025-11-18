using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JobFitScoreAPI.Services
{
    public class VagaHabilidadeService
    {
        private readonly AppDbContext _context;

        public VagaHabilidadeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VagaHabilidade>> GetHabilidadesByVagaIdAsync(int vagaId)
        {
            return await _context.VagaHabilidades
                .Include(vh => vh.Habilidade)
                .Where(vh => vh.VagaId == vagaId)
                .ToListAsync();
        }

        public async Task<VagaHabilidade> AddHabilidadeToVagaAsync(int vagaId, int habilidadeId)
        {
            var vaga = await _context.Vagas.FindAsync(vagaId);
            if (vaga == null)
                throw new KeyNotFoundException("Vaga não encontrada.");

            var habilidade = await _context.Habilidades.FindAsync(habilidadeId);
            if (habilidade == null)
                throw new KeyNotFoundException("Habilidade não encontrada.");

            var existing = await _context.VagaHabilidades
                .FirstOrDefaultAsync(vh => vh.VagaId == vagaId && vh.HabilidadeId == habilidadeId);

            if (existing != null)
                throw new InvalidOperationException("Vaga já possui esta habilidade.");

            var vagaHabilidade = new VagaHabilidade
            {
                VagaId = vagaId,
                HabilidadeId = habilidadeId
            };

            _context.VagaHabilidades.Add(vagaHabilidade);
            await _context.SaveChangesAsync();

            return vagaHabilidade;
        }

        public async Task<bool> RemoveHabilidadeFromVagaAsync(int vagaId, int habilidadeId)
        {
            var vagaHabilidade = await _context.VagaHabilidades
                .FirstOrDefaultAsync(vh => vh.VagaId == vagaId && vh.HabilidadeId == habilidadeId);

            if (vagaHabilidade == null)
                return false;

            _context.VagaHabilidades.Remove(vagaHabilidade);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
