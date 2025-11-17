using Microsoft.AspNetCore.Mvc;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Services;
using Asp.Versioning;

namespace JobFitScoreAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/ml")]
    [Asp.Versioning.ApiVersion("2.0")]
    public class MlController : ControllerBase
    {
        private readonly JobFitMLService _mlService;

        public MlController(JobFitMLService mlService)
        {
            _mlService = mlService;
        }

        [HttpPost("prever")]
        public IActionResult PreverCompatibilidade([FromBody] JobFitData entrada)
        {
            if (entrada == null)
                return BadRequest(new { mensagem = "Dados de entrada inválidos." });

            float resultado = _mlService.PreverCompatibilidade(entrada);
            return Ok(new
            {
                Score = resultado,
                Mensagem = $"Score de compatibilidade calculado com sucesso: {resultado:F2}%"
            });
        }
    }
}
