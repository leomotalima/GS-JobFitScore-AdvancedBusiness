using Microsoft.ML;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Services
{
    public class JobFitMLService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;

      
        public JobFitMLService()
        {
            _mlContext = new MLContext();

            // Carrega o dataset de treinamento
            var dataView = _mlContext.Data.LoadFromTextFile<JobFitData>(
                path: "Scripts/ml_jobfitscore.csv",
                hasHeader: true,
                separatorChar: ',');

            // Define o pipeline de transformação e treinamento
            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(JobFitData.ExperienciaAnos),
                    nameof(JobFitData.HabilidadesMatch),
                    nameof(JobFitData.CursosRelacionados),
                    nameof(JobFitData.NivelVaga))
                .Append(_mlContext.Regression.Trainers.Sdca(
                    labelColumnName: "ScoreCompatibilidade",
                    maximumNumberOfIterations: 100));

            // Treina o modelo
            _model = pipeline.Fit(dataView);
        }

        // Método que usa o modelo treinado para prever o score de compatibilidade
        public float PreverCompatibilidade(JobFitData dadosEntrada)
        {
            var engine = _mlContext.Model.CreatePredictionEngine<JobFitData, JobFitPrediction>(_model);
            var resultado = engine.Predict(dadosEntrada);
            return resultado.ScoreCompatibilidade;
        }
    }

    // Classe auxiliar que representa o resultado da previsão
    public class JobFitPrediction
    {
        public float ScoreCompatibilidade { get; set; }
    }
}
