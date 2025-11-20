using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JobFitScoreAPI.Services
{
    public class CursoService
    {
        private readonly AppDbContext _context;

        public CursoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Curso>> GetAllCursosAsync()
        {
            return await _context.Cursos.ToListAsync();
        }

        public async Task<Curso?> GetCursoByIdAsync(int id)
        {
            return await _context.Cursos.FindAsync(id);
        }

        public async Task<Curso> CreateCursoAsync(Curso curso)
        {
            if (curso == null)
                throw new ArgumentNullException(nameof(curso));

            if (string.IsNullOrWhiteSpace(curso.Nome))
                throw new ArgumentException("Nome do curso é obrigatório");

            if (curso.UsuarioId <= 0)
                throw new ArgumentException("UsuarioId é obrigatório");

            var usuario = await _context.Usuarios.FindAsync(curso.UsuarioId);
            if (usuario == null)
                throw new InvalidOperationException("Usuário não encontrado");

            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            return curso;
        }

        public async Task<Curso?> UpdateCursoAsync(int id, Curso curso)
        {
            if (curso == null)
                throw new ArgumentNullException(nameof(curso));

            var cursoExistente = await _context.Cursos.FindAsync(id);
            if (cursoExistente == null)
                return null;

            if (!string.IsNullOrWhiteSpace(curso.Nome))
                cursoExistente.Nome = curso.Nome;

            if (!string.IsNullOrWhiteSpace(curso.Instituicao))
                cursoExistente.Instituicao = curso.Instituicao;

            if (curso.DataConclusao.HasValue)
                cursoExistente.DataConclusao = curso.DataConclusao;

            if (!string.IsNullOrWhiteSpace(curso.Descricao))
                cursoExistente.Descricao = curso.Descricao;

            _context.Cursos.Update(cursoExistente);
            await _context.SaveChangesAsync();

            return cursoExistente;
        }

        public async Task<bool> DeleteCursoAsync(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
                return false;

            _context.Cursos.Remove(curso);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Curso>> GetCursosByUsuarioAsync(int usuarioId)
        {
            return await _context.Cursos
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Curso>> SearchCursosAsync(string? nome, string? instituicao)
        {
            var query = _context.Cursos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(c => c.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(instituicao))
                query = query.Where(c => c.Instituicao != null && c.Instituicao.Contains(instituicao));

            return await query.ToListAsync();
        }
    }
}
