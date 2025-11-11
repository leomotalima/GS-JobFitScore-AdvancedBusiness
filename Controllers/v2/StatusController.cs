using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace JobFitScoreApi.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/status")]
    public class StatusController : ControllerBase
    {
        [HttpGet("info")]
        public IActionResult GetStatusInfo()
        {
            return Ok(new
            {
                mensagem = "JobFitScore API v2 ativa 🚀",
                versao = "2.0",
                ambiente = Environment.MachineName,
                horario = DateTime.UtcNow
            });
        }
    }
}
