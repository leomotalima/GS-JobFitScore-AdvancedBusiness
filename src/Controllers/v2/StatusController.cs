using Microsoft.AspNetCore.Mvc;   
using Asp.Versioning;              
using Microsoft.EntityFrameworkCore; 
using JobFitScoreAPI.Data;         
using JobFitScoreAPI.Models;


namespace JobFitScoreAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/status")]
    [Asp.Versioning.ApiVersion("2.0")]
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
