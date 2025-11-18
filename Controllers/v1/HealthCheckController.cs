using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class HealthCheckController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(
            HealthCheckService healthCheckService,
            ILogger<HealthCheckController> logger)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }

        /// <summary>
        /// Verifica o status de saúde da aplicação
        /// </summary>
        /// <returns>Status detalhado dos health checks</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var healthReport = await _healthCheckService.CheckHealthAsync();

                var response = new
                {
                    status = healthReport.Status.ToString(),
                    totalDuration = healthReport.TotalDuration.TotalMilliseconds,
                    checks = healthReport.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description,
                        duration = entry.Value.Duration.TotalMilliseconds,
                        exception = entry.Value.Exception?.Message,
                        data = entry.Value.Data
                    }),
                    timestamp = DateTime.UtcNow
                };

                _logger.LogInformation("Health check executado: {Status}", healthReport.Status);

                return healthReport.Status == HealthStatus.Healthy
                    ? Ok(response)
                    : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar health check");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    status = "Error",
                    message = "Erro ao verificar status da aplicação",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Verifica apenas o status básico da aplicação (liveness probe)
        /// </summary>
        /// <returns>Status OK se a aplicação está rodando</returns>
        [HttpGet("live")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Live()
        {
            return Ok(new
            {
                status = "Alive",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Verifica se a aplicação está pronta para receber requisições (readiness probe)
        /// </summary>
        /// <returns>Status da prontidão da aplicação</returns>
        [HttpGet("ready")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Ready()
        {
            try
            {
                var healthReport = await _healthCheckService.CheckHealthAsync();

                var isReady = healthReport.Status == HealthStatus.Healthy;

                var response = new
                {
                    status = isReady ? "Ready" : "NotReady",
                    timestamp = DateTime.UtcNow
                };

                return isReady
                    ? Ok(response)
                    : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar readiness");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    status = "NotReady",
                    message = "Erro ao verificar prontidão da aplicação",
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}