using Microsoft.AspNetCore.Mvc;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class VagaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public VagaController(AppDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Vagas.ToList());

        [HttpPost]
        public IActionResult Create(Vaga vaga)
        {
            _context.Vagas.Add(vaga);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAll), vaga);
        }
    }
}
