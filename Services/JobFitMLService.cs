using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using Microsoft.ML;


namespace JobFitScoreAPI.Services
{
    public class JobFitMLService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;

      
        public JobFitMLService()
        {
            _mlContext = new MLContext();

            var dataView = _mlContext.Data.LoadFromTextFile<JobFitData>(
                path: "Scripts/ml_jobfitscore.csv",
                hasHeader: true,
                separatorChar: ',');

            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(JobFitData.ExperienciaAnos),
                    nameof(JobFitData.HabilidadesMatch),
                    nameof(JobFitData.CursosRelacionados),
                    nameof(JobFitData.NivelVaga))
                .Append(_mlContext.Regression.Trainers.Sdca(
                    labelColumnName: "ScoreCompatibilidade",
                    maximumNumberOfIterations: 100));

            _model = pipeline.Fit(dataView);
        }

        public float PreverCompatibilidade(JobFitData dadosEntrada)
        {
            var engine = _mlContext.Model.CreatePredictionEngine<JobFitData, JobFitPrediction>(_model);
            var resultado = engine.Predict(dadosEntrada);
            return resultado.ScoreCompatibilidade;
        }
    }

    public class JobFitPrediction
    {
        public float ScoreCompatibilidade { get; set; }
    }
}
