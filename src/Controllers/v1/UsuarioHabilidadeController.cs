using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Dtos.UsuarioHabilidade;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/usuario-habilidade")]
    [Tags("Usuários Habilidades")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize]
    public class UsuarioHabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioHabilidadeController(AppDbContext context)
        {
            _context = context;
        }

        // Classe de resposta padronizada
        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public T? Data { get; set; }

            public static ApiResponse<T> Ok(T? data, string message = "") =>
                new ApiResponse<T> { Success = true, Message = message, Data = data };

            public static ApiResponse<T> Fail(string message) =>
                new ApiResponse<T> { Success = false, Message = message };
        }

        // GET - Listar habilidades de um usuário
        [HttpGet("{usuarioId}", Name = "GetHabilidadesDoUsuario")]
        [SwaggerOperation(Summary = "Lista todas as habilidades de um usuário")]
        [SwaggerResponse(StatusCodes.Status200OK, "Habilidades retornadas com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nenhuma habilidade encontrada para o usuário")]
        public async Task<IActionResult> GetHabilidadesDoUsuario(int usuarioId)
        {
            var usuarioHabilidades = await _context.UsuarioHabilidades
                .Include(uh => uh.Habilidade)
                .Include(uh => uh.Usuario)
                .Where(uh => uh.Usuario != null && uh.Habilidade != null && uh.Usuario.IdUsuario == usuarioId)
                .ToListAsync();

            if (!usuarioHabilidades.Any())
                return NotFound(ApiResponse<string>.Fail("Nenhuma habilidade encontrada para o usuário."));

            var resultado = usuarioHabilidades.Select(uh => new
            {
                UsuarioId = uh.Usuario!.IdUsuario,
                HabilidadeId = uh.Habilidade!.IdHabilidade,
                HabilidadeNome = uh.Habilidade.Nome
            });

            return Ok(ApiResponse<object>.Ok(resultado, "Habilidades listadas com sucesso."));
        }

        // POST - Adicionar habilidade a um usuário
        [HttpPost(Name = "AdicionarHabilidadeUsuario")]
        [SwaggerOperation(Summary = "Adiciona uma habilidade a um usuário")]
        [SwaggerResponse(StatusCodes.Status201Created, "Habilidade adicionada com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Input inválido")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Usuário ou habilidade não encontrado")]
        public async Task<IActionResult> AdicionarHabilidade([FromBody] UsuarioHabilidadeInput dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<string>.Fail("Input não pode ser nulo."));

            var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
            var habilidade = await _context.Habilidades.FindAsync(dto.IdHabilidade);

            if (usuario == null || habilidade == null)
                return NotFound(ApiResponse<string>.Fail("Usuário ou habilidade não encontrado."));

            var usuarioHabilidadeExistente = await _context.UsuarioHabilidades
                .FirstOrDefaultAsync(uh => uh.Usuario != null && uh.Habilidade != null &&
                                           uh.Usuario.IdUsuario == dto.IdUsuario &&
                                           uh.Habilidade.IdHabilidade == dto.IdHabilidade);

            if (usuarioHabilidadeExistente != null)
                return BadRequest(ApiResponse<string>.Fail("Habilidade já cadastrada para este usuário."));

            var usuarioHabilidade = new UsuarioHabilidade
            {
                Usuario = usuario,
                Habilidade = habilidade
            };

            _context.UsuarioHabilidades.Add(usuarioHabilidade);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHabilidadesDoUsuario),
                new { usuarioId = usuario.IdUsuario },
                ApiResponse<UsuarioHabilidade>.Ok(usuarioHabilidade, "Habilidade adicionada com sucesso."));
        }

        // DELETE - Remover habilidade de um usuário
        [HttpDelete(Name = "RemoverHabilidadeUsuario")]
        [SwaggerOperation(Summary = "Remove uma habilidade de um usuário")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Habilidade removida com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Habilidade não encontrada para o usuário")]
        public async Task<IActionResult> RemoverHabilidade([FromBody] UsuarioHabilidadeInput dto)
        {
            var usuarioHabilidade = await _context.UsuarioHabilidades
                .Include(uh => uh.Usuario)
                .Include(uh => uh.Habilidade)
                .Where(uh => uh.Usuario != null && uh.Habilidade != null &&
                             uh.Usuario.IdUsuario == dto.IdUsuario &&
                             uh.Habilidade.IdHabilidade == dto.IdHabilidade)
                .FirstOrDefaultAsync();

            if (usuarioHabilidade == null)
                return NotFound(ApiResponse<string>.Fail("Habilidade não encontrada para o usuário."));

            _context.UsuarioHabilidades.Remove(usuarioHabilidade);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
