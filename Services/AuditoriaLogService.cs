using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JobFitScoreAPI.Services
{
    public class AuditoriaLogService
    {
        private readonly AppDbContext _context;

        public AuditoriaLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string nomeTabela, string operacao, int? registroId, string? usuarioBanco, string? detalhe)
        {
            var log = new AuditoriaLog
            {
                NomeTabela = nomeTabela,
                Operacao = operacao,
                RegistroId = registroId,
                UsuarioBanco = usuarioBanco,
                DataOperacao = DateTime.Now,
                Detalhe = detalhe
            };

            _context.AuditoriaLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditoriaLog>> GetLogsAsync()
        {
            return await _context.AuditoriaLogs
                .OrderByDescending(l => l.DataOperacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditoriaLog>> GetLogsByTabelaAsync(string nomeTabela)
        {
            return await _context.AuditoriaLogs
                .Where(l => l.NomeTabela == nomeTabela)
                .OrderByDescending(l => l.DataOperacao)
                .ToListAsync();
        }
    }
}
