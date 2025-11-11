using Microsoft.AspNetCore.Mvc;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CursoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CursoController(AppDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Cursos.ToList());

        [HttpPost]
        public IActionResult Create(Curso curso)
        {
            _context.Cursos.Add(curso);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAll), curso);
        }
    }
}
