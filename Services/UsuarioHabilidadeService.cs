using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JobFitScoreAPI.Services
{
    public class UsuarioHabilidadeService
    {
        private readonly AppDbContext _context;

        public UsuarioHabilidadeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuarioHabilidade>> GetHabilidadesByUsuarioIdAsync(int usuarioId)
        {
            return await _context.UsuarioHabilidades
                .Include(uh => uh.Habilidade)
                .Where(uh => uh.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<UsuarioHabilidade> AddHabilidadeToUsuarioAsync(int usuarioId, int habilidadeId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            var habilidade = await _context.Habilidades.FindAsync(habilidadeId);
            if (habilidade == null)
                throw new KeyNotFoundException("Habilidade não encontrada.");

            var existing = await _context.UsuarioHabilidades
                .FirstOrDefaultAsync(uh => uh.UsuarioId == usuarioId && uh.HabilidadeId == habilidadeId);

            if (existing != null)
                throw new InvalidOperationException("Usuário já possui esta habilidade.");

            var usuarioHabilidade = new UsuarioHabilidade
            {
                UsuarioId = usuarioId,
                HabilidadeId = habilidadeId
            };

            _context.UsuarioHabilidades.Add(usuarioHabilidade);
            await _context.SaveChangesAsync();

            return usuarioHabilidade;
        }

        public async Task<bool> RemoveHabilidadeFromUsuarioAsync(int usuarioId, int habilidadeId)
        {
            var usuarioHabilidade = await _context.UsuarioHabilidades
                .FirstOrDefaultAsync(uh => uh.UsuarioId == usuarioId && uh.HabilidadeId == habilidadeId);

            if (usuarioHabilidade == null)
                return false;

            _context.UsuarioHabilidades.Remove(usuarioHabilidade);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
