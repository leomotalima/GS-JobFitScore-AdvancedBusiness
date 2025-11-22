using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Dtos.UsuarioHabilidade;
using Swashbuckle.AspNetCore.Annotations;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/usuario-habilidade")]
    [Tags("UsuariosHabilidades")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize]
    public class UsuarioHabilidadeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;

        public UsuarioHabilidadeController(AppDbContext context, LinkGenerator linkGenerator)
        {
            _context = context;
            _linkGenerator = linkGenerator;
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

        // GET - Habilidades de um usuário
        [HttpGet("{usuarioId}", Name = "GetHabilidadesDoUsuario")]
        [SwaggerOperation(Summary = "Lista habilidades de um usuário")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHabilidadesDoUsuario(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                return NotFound(ApiResponse<string>.Fail("Usuário não encontrado."));

            var usuarioHabilidades = await _context.UsuarioHabilidades
                .Include(uh => uh.Habilidade)
                .Where(uh => uh.Usuario!.IdUsuario == usuarioId)
                .ToListAsync();

            var habilidades = usuarioHabilidades.Select(uh => new
            {
                uh.Habilidade!.IdHabilidade,
                uh.Habilidade.Nome
            });

            var result = new
            {
                usuarioId,
                habilidades,
                links = new List<object>
                {
                    new { rel = "self", href = GetUserSkillsUrl(usuarioId), method = "GET" },
                    new { rel = "add", href = GetAddSkillUrl(), method = "POST" },
                    new { rel = "remove", href = GetRemoveSkillUrl(), method = "DELETE" }
                }
            };

            return Ok(ApiResponse<object>.Ok(result, "Habilidades listadas com sucesso."));
        }

        // POST - Adicionar habilidade ao usuário
        [HttpPost(Name = "AdicionarHabilidadeUsuario")]
        [SwaggerOperation(Summary = "Adiciona habilidade ao usuário")]
        [SwaggerResponse(StatusCodes.Status201Created)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AdicionarHabilidade([FromBody] UsuarioHabilidadeInput dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<string>.Fail("Input inválido."));

            var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
            var habilidade = await _context.Habilidades.FindAsync(dto.IdHabilidade);

            if (usuario == null || habilidade == null)
                return NotFound(ApiResponse<string>.Fail("Usuário ou habilidade não encontrado."));

            var existente = await _context.UsuarioHabilidades
                .FirstOrDefaultAsync(uh => uh.Usuario!.IdUsuario == dto.IdUsuario &&
                                           uh.Habilidade!.IdHabilidade == dto.IdHabilidade);

            if (existente != null)
                return BadRequest(ApiResponse<string>.Fail("Habilidade já cadastrada para este usuário."));

            var registro = new UsuarioHabilidade
            {
                Usuario = usuario,
                Habilidade = habilidade
            };

            _context.UsuarioHabilidades.Add(registro);
            await _context.SaveChangesAsync();

            var result = new
            {
                usuario.IdUsuario,
                habilidade.IdHabilidade,
                habilidade.Nome,
                links = new List<object>
                {
                    new { rel = "self", href = GetUserSkillsUrl(usuario.IdUsuario), method = "GET" },
                    new { rel = "remove", href = GetRemoveSkillUrl(), method = "DELETE" }
                }
            };

            return CreatedAtAction(nameof(GetHabilidadesDoUsuario),
                new { usuarioId = usuario.IdUsuario },
                ApiResponse<object>.Ok(result, "Habilidade adicionada com sucesso."));
        }

        // DELETE - Remover habilidade
        [HttpDelete(Name = "RemoverHabilidadeUsuario")]
        [SwaggerOperation(Summary = "Remove habilidade do usuário")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoverHabilidade([FromBody] UsuarioHabilidadeInput dto)
        {
            var usuarioHabilidade = await _context.UsuarioHabilidades
                .Include(uh => uh.Usuario)
                .Include(uh => uh.Habilidade)
                .FirstOrDefaultAsync(uh =>
                    uh.Usuario!.IdUsuario == dto.IdUsuario &&
                    uh.Habilidade!.IdHabilidade == dto.IdHabilidade);

            if (usuarioHabilidade == null)
                return NotFound(ApiResponse<string>.Fail("Habilidade não encontrada para o usuário."));

            _context.UsuarioHabilidades.Remove(usuarioHabilidade);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // HATEOAS Helper URLs
        private string GetUserSkillsUrl(int usuarioId) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetHabilidadesDoUsuario), "UsuarioHabilidade",
                new { usuarioId }) ?? string.Empty;

        private string GetAddSkillUrl() =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(AdicionarHabilidade), "UsuarioHabilidade") ?? string.Empty;

        private string GetRemoveSkillUrl() =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(RemoverHabilidade), "UsuarioHabilidade") ?? string.Empty;
    }
}
