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

            // Cria os dados de entrada para o modelo ML
            var dadosEntrada = new JobFitData
            {
                ExperienciaAnos = 3, // exemplo (poderia vir do usuário)
                HabilidadesMatch = CalcularHabilidadesMatch(usuario, vaga),
                CursosRelacionados = 1, // exemplo
                NivelVaga = 2, // exemplo
                ScoreCompatibilidade = 0 // campo usado apenas no treinamento
            };

            float score = _mlService.PreverCompatibilidade(dadosEntrada);

            // Cria a candidatura
            var candidatura = new Candidatura
            {
                UsuarioId = usuarioId,
                VagaId = vagaId
                // Score e DataCandidatura não existem no model, então não adicionamos
            };

            _context.Candidaturas.Add(candidatura);
            _context.SaveChanges();

            return score;
        }

        private int CalcularHabilidadesMatch(Usuario usuario, Vaga vaga)
        {
            // Aqui você precisa acessar as habilidades do usuário e da vaga
            // Se você ainda não tem uma tabela de relacionamento, o método precisa ser ajustado
            return 0; // placeholder para evitar erro
        }
    }
}
