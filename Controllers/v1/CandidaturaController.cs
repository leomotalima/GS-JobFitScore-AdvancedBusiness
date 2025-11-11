using Microsoft.AspNetCore.Mvc;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;

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

        [HttpGet]
        public IActionResult GetAll()
        {
            var candidaturas = _context.Candidaturas
                .Select(c => new
                {
                    c.Id,
                    Usuario = c.Usuario.Nome,
                    Vaga = c.Vaga.Titulo,
                    c.Score,
                    c.DataCandidatura
                })
                .ToList();

            return Ok(candidaturas);
        }
    }
}
