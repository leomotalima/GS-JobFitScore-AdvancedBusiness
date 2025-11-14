using JobFitScoreAPI.Data;
using JobFitScoreAPI.Services;
using JobFitScoreAPI.Models;
using System;
using System.Linq;
using System.Collections.Generic;

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

        public double ProcessarCandidatura(int idUsuario, int idVaga)
        {
            var usuario = _context.Usuarios.Find(idUsuario);
            var vaga = _context.Vagas.Find(idVaga);

           
            if (usuario == null || vaga == null)
                throw new Exception("Usuário ou vaga não encontrada.");

            // Cria os dados de entrada para o modelo ML
            var dadosEntrada = new JobFitData
            {
                ExperienciaAnos = 3, // exemplo (poderia vir do usuário)
                HabilidadesMatch = CalcularHabilidadesMatch(usuario.Habilidades, vaga.Habilidades),
                CursosRelacionados = 1, // exemplo
                NivelVaga = 2, // exemplo
                ScoreCompatibilidade = 0 // campo usado apenas no treinamento
            };

            
            float score = _mlService.PreverCompatibilidade(dadosEntrada);

           
            var candidatura = new Candidatura
            {
                IdUsuario = idUsuario,
                IdVaga = idVaga,
                Score = (int?)score,
                DataCandidatura = DateTime.Now
            };

            _context.Candidaturas.Add(candidatura);
            _context.SaveChanges();

            return score;
        }

        private int CalcularHabilidadesMatch(string? habilidadesUsuario, string? habilidadesVaga)
        {
            if (string.IsNullOrEmpty(habilidadesUsuario) || string.IsNullOrEmpty(habilidadesVaga))
                return 0;

            var userSkills = habilidadesUsuario.Split(',').Select(h => h.Trim()).ToList();
            var jobSkills = habilidadesVaga.Split(',').Select(h => h.Trim()).ToList();

            return userSkills.Intersect(jobSkills).Count();
        }
    }
}
