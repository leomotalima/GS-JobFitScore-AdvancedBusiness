using Microsoft.AspNetCore.Mvc;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Services;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MlController : ControllerBase
    {
        private readonly JobFitMlService _mlService;

        public MlController(JobFitMlService mlService)
        {
            _mlService = mlService;
        }

        /// <summary>
        /// Faz uma previsão de compatibilidade profissional com base nos dados fornecidos.
        /// </summary>
        /// <param name="input">Dados do candidato e vaga</param>
        /// <returns>Score previsto (0 a 100)</returns>
        [HttpPost("prever")]
        public IActionResult Prever([FromBody] JobFitData input)
        {
            if (input == null)
                return BadRequest(new { mensagem = "Dados de entrada inválidos." });

            var resultado = _mlService.PreverCompatibilidade(input);
            return Ok(new
            {
                score = resultado,
                mensagem = "Previsão de compatibilidade gerada com sucesso!"
            });
        }
    }
}
