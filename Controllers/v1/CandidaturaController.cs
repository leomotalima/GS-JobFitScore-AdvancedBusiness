using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CandidaturaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CandidaturaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/candidatura
        [HttpGet]
        public IActionResult GetAll()
        {
            var candidaturas = _context.Candidaturas
                .Include(c => c.Usuario)
                .Include(c => c.Vaga)
                .Select(c => new
                {
                    c.IdCandidatura,
                    Usuario = c.Usuario != null ? c.Usuario.Nome : "Usuário não definido",
                    Vaga = c.Vaga != null ? c.Vaga.Titulo : "Vaga não definida",
                    c.Score,
                    c.DataCandidatura
                })
                .ToList();

            return Ok(candidaturas);
        }

        // GET: api/v1/candidatura/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var candidatura = _context.Candidaturas
                .Include(c => c.Usuario)
                .Include(c => c.Vaga)
                .FirstOrDefault(c => c.IdCandidatura == id);

            if (candidatura == null)
                return NotFound(new { mensagem = "Candidatura não encontrada." });

            return Ok(new
            {
                candidatura.IdCandidatura,
                Usuario = candidatura.Usuario != null ? candidatura.Usuario.Nome : "Usuário não definido",
                Vaga = candidatura.Vaga != null ? candidatura.Vaga.Titulo : "Vaga não definida",
                candidatura.Score,
                candidatura.DataCandidatura
            });
        }

        // POST: api/v1/candidatura
        [HttpPost]
        public IActionResult Create([FromBody] Candidatura candidatura)
        {
            if (candidatura == null)
                return BadRequest(new { mensagem = "Dados inválidos." });

            _context.Candidaturas.Add(candidatura);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById),
                new { id = candidatura.IdCandidatura },
                candidatura);
        }
    }
}
