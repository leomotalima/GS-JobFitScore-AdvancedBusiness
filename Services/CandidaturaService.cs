using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using System;
using System.Linq;

namespace JobFitScoreAPI.Services
{
    public class CandidaturaService
    {
        private readonly AppDbContext _context;
        private readonly JobFitMLService _mlService;

        public CandidaturaService(AppDbContext context, JobFitMLService mlService)
        {
            _context = context;
            _mlService = mlService;
        }

        public double ProcessarCandidatura(int usuarioId, int vagaId)
        {
            var usuario = _context.Usuarios.Find(usuarioId);
            var vaga = _context.Vagas.Find(vagaId);

            if (usuario == null || vaga == null)
                throw new Exception("Usuário ou vaga não encontrada.");

            
            var dadosEntrada = new JobFitData
            {
                ExperienciaAnos = 3, 
                HabilidadesMatch = CalcularHabilidadesMatch(usuario, vaga),
                CursosRelacionados = 1, 
                NivelVaga = 2, 
                ScoreCompatibilidade = 0             };

            float score = _mlService.PreverCompatibilidade(dadosEntrada);

            
            var candidatura = new Candidatura
            {
                UsuarioId = usuarioId,
                VagaId = vagaId
                
            };

            _context.Candidaturas.Add(candidatura);
            _context.SaveChanges();

            return score;
        }

        private int CalcularHabilidadesMatch(Usuario usuario, Vaga vaga)
        {
            return 0; 
        }
    }
}
