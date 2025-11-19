using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobFitScoreAPI.Services
{
    public class CandidaturaService
    {
        private readonly ICandidaturaRepository _candidaturaRepository;
        private readonly AppDbContext _context;
        private readonly JobFitMLService _mlService;

        public CandidaturaService(ICandidaturaRepository candidaturaRepository, AppDbContext context, JobFitMLService mlService)
        {
            _candidaturaRepository = candidaturaRepository;
            _context = context;
            _mlService = mlService;
        }

        public async Task<double> ProcessarCandidaturaAsync(int usuarioId, int vagaId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            var vaga = await _context.Vagas.FindAsync(vagaId);

            if (usuario == null || vaga == null)
                throw new Exception("Usuário ou vaga não encontrada.");

            // Verificar se já existe candidatura
            var candidaturas = await _candidaturaRepository.GetAllAsync();
            if (candidaturas.Any(c => c.UsuarioId == usuarioId && c.VagaId == vagaId))
                 throw new Exception("Usuário já candidatado a esta vaga.");

            int matchHabilidades = await CalcularHabilidadesMatchAsync(usuarioId, vagaId);

            var dadosEntrada = new JobFitData
            {
                ExperienciaAnos = 3, 
                HabilidadesMatch = matchHabilidades,
                CursosRelacionados = 1, 
                NivelVaga = 2, 
                ScoreCompatibilidade = 0
            };

            float score = _mlService.PreverCompatibilidade(dadosEntrada);

            var candidatura = new Candidatura
            {
                UsuarioId = usuarioId,
                VagaId = vagaId,
                DataCandidatura = DateTime.Now,
                Status = "Em Análise"
            };

            await _candidaturaRepository.AddAsync(candidatura);
            await _candidaturaRepository.SaveChangesAsync();

            return score;
        }

        private async Task<int> CalcularHabilidadesMatchAsync(int usuarioId, int vagaId)
        {
            var usuarioHabilidades = await _context.UsuarioHabilidades
                .Where(uh => uh.UsuarioId == usuarioId)
                .Select(uh => uh.HabilidadeId)
                .ToListAsync();

            var vagaHabilidades = await _context.VagaHabilidades
                .Where(vh => vh.VagaId == vagaId)
                .Select(vh => vh.HabilidadeId)
                .ToListAsync();

            if (!vagaHabilidades.Any()) return 100; 
            
            int matches = usuarioHabilidades.Intersect(vagaHabilidades).Count();
            
            return matches;
        }

        public async Task<IEnumerable<Candidatura>> GetCandidaturasByUsuarioAsync(int usuarioId)
        {
            var candidaturas = await _candidaturaRepository.GetAllAsync();
            return candidaturas.Where(c => c.UsuarioId == usuarioId);
        }

        public async Task<IEnumerable<Candidatura>> GetCandidaturasByVagaAsync(int vagaId)
        {
            var candidaturas = await _candidaturaRepository.GetAllAsync();
            return candidaturas.Where(c => c.VagaId == vagaId);
        }
        
        public async Task<Candidatura?> GetCandidaturaByIdAsync(int id)
        {
            return await _candidaturaRepository.GetByIdAsync(id);
        }
    }
}
