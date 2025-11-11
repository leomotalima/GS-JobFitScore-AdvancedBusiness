using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuditoriaLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditoriaLogController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditoriaLog>>> GetAll()
        {
            return await _context.AuditoriaLogs
                .OrderByDescending(a => a.DataOperacao)
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuditoriaLog>> GetById(int id)
        {
            var log = await _context.AuditoriaLogs.FindAsync(id);
            if (log == null) return NotFound();
            return Ok(log);
        }
    }
}
