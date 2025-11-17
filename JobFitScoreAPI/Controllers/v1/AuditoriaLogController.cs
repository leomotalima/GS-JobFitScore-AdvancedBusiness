using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auditoria")]
    [Tags("Auditoria de Logs")]
    [Produces("application/json")]
    [Authorize]
    public class AuditoriaLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditoriaLogController(AppDbContext context)
        {
            _context = context;
        }

        // Classe de resposta padrão
        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public T? Data { get; set; }

            public static ApiResponse<T> Ok(T data, string message = "") =>
                new ApiResponse<T> { Success = true, Message = message, Data = data };

            public static ApiResponse<T> Fail(string message) =>
                new ApiResponse<T> { Success = false, Message = message };
        }

        // GET: api/v1/auditoria?page=1&pageSize=10
        [HttpGet]
        [SwaggerOperation(Summary = "Lista os registros de auditoria", Description = "Retorna uma lista paginada de logs de auditoria")]
        [SwaggerResponse(StatusCodes.Status200OK, "Logs retornados com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Parâmetros inválidos")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno no servidor")]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(ApiResponse<string>.Fail("Parâmetros de paginação inválidos."));

            var totalItems = await _context.AuditoriaLogs.CountAsync();

            var logs = await _context.AuditoriaLogs
                .OrderByDescending(a => a.DataOperacao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new
                {
                    l.IdAuditoria,
                    l.NomeTabela,
                    l.Operacao,
                    l.RegistroId,
                    l.UsuarioBanco,
                    l.DataOperacao,
                    l.Detalhe
                })
                .ToListAsync();

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(ApiResponse<object>.Ok(new { meta, data = logs }, "Logs retornados com sucesso."));
        }

        // GET: api/v1/auditoria/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Busca um log pelo ID", Description = "Retorna os detalhes de um registro de auditoria")]
        [SwaggerResponse(StatusCodes.Status200OK, "Log encontrado com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Log não encontrado")]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await _context.AuditoriaLogs
                .Where(l => l.IdAuditoria == id)
                .Select(l => new
                {
                    l.IdAuditoria,
                    l.NomeTabela,
                    l.Operacao,
                    l.RegistroId,
                    l.UsuarioBanco,
                    l.DataOperacao,
                    l.Detalhe
                })
                .FirstOrDefaultAsync();

            if (log == null)
                return NotFound(ApiResponse<string>.Fail("Log não encontrado."));

            return Ok(ApiResponse<object>.Ok(log, "Log encontrado com sucesso."));
        }
    }
}
