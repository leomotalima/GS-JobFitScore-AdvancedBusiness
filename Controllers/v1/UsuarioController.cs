using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Dtos.Usuario;
using JobFitScoreAPI.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace JobFitScoreAPI.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/usuarios")] 
    [Tags("Usuários")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LinkGenerator _linkGenerator;
        private readonly ICryptoService _crypto;


        public UsuarioController(AppDbContext context, LinkGenerator linkGenerator, ICryptoService crypto)
        {
            _context = context;
            _linkGenerator = linkGenerator;
            _crypto = crypto;
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

        // GET - Listar usuários
        [HttpGet(Name = "GetUsuarios")]
        [SwaggerOperation(Summary = "Lista todos os usuários")]
        [SwaggerResponse(StatusCodes.Status200OK, "Lista de usuários retornada com sucesso")]
        public async Task<IActionResult> GetUsuarios(int page = 1, int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);

            var totalItems = await _context.Usuarios.CountAsync();

            var usuarios = await _context.Usuarios
                .OrderBy(u => u.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UsuarioOutput
                {
                    IdUsuario = u.IdUsuario,
                    Nome = u.Nome,
                    Email = u.Email
                })
                .ToListAsync();

            var meta = new
            {
                totalItems,
                page,
                pageSize,
                totalPages = Math.Ceiling((double)totalItems / pageSize)
            };

            var result = new
            {
                meta,
                data = usuarios,
                links = new List<object>
                {
                    new { rel = "self", href = GetPageUrl(page, pageSize), method = "GET" },
                    new { rel = "next", href = GetPageUrl(page + 1, pageSize), method = "GET" },
                    new { rel = "previous", href = GetPageUrl(page - 1, pageSize), method = "GET" }
                }
            };

            return Ok(ApiResponse<object>.Ok(result, "Usuários listados com sucesso."));
        }

        // GET - Buscar usuário por ID
        [HttpGet("{id}", Name = "GetUsuario")]
        [SwaggerOperation(Summary = "Obtém um usuário específico")]
        [SwaggerResponse(StatusCodes.Status200OK, "Usuário encontrado com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Usuário não encontrado")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.IdUsuario == id)
                .Select(u => new UsuarioOutput
                {
                    IdUsuario = u.IdUsuario,
                    Nome = u.Nome,
                    Email = u.Email
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound(ApiResponse<string>.Fail("Usuário não encontrado."));

            var result = new
            {
                usuario,
                links = new List<object>
                {
                    new { rel = "self", href = GetByIdUrl(id), method = "GET" },
                    new { rel = "update", href = GetByIdUrl(id), method = "PUT" },
                    new { rel = "delete", href = GetByIdUrl(id), method = "DELETE" },
                    new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }
                }
            };

            return Ok(ApiResponse<object>.Ok(result, "Usuário encontrado com sucesso."));
        }

        // POST - Criar usuário (Aberto)
        [AllowAnonymous]
        [HttpPost(Name = "CreateUsuario")]
        [SwaggerOperation(Summary = "Cria um novo usuário")]
        [SwaggerResponse(StatusCodes.Status201Created, "Usuário criado com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro na requisição")]
        public async Task<IActionResult> CreateUsuario([FromBody] UsuarioInput input)
        {
            if (input == null)
                return BadRequest(ApiResponse<string>.Fail("Input não pode ser nulo."));

            var usuario = new Usuario
            {
                Nome = input.Nome,
                Email = input.Email,
                Senha = _crypto.HashPassword(input.Senha)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var output = new UsuarioOutput
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.Nome,
                Email = usuario.Email
            };

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario },
                ApiResponse<UsuarioOutput>.Ok(output, "Usuário criado com sucesso."));
        }

        // PUT - Atualizar usuário
        [HttpPut("{id}", Name = "UpdateUsuario")]
        [SwaggerOperation(Summary = "Atualiza um usuário existente")]
        [SwaggerResponse(StatusCodes.Status200OK, "Usuário atualizado com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Usuário não encontrado")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioUpdateInput input)
        {
            if (input == null)
                return BadRequest(ApiResponse<string>.Fail("Input não pode ser nulo."));

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(ApiResponse<string>.Fail("Usuário não encontrado."));

            usuario.Nome = input.Nome ?? usuario.Nome;
            usuario.Email = input.Email ?? usuario.Email;

            if (!string.IsNullOrWhiteSpace(input.Senha))
                usuario.Senha = _crypto.HashPassword(input.Senha);

            await _context.SaveChangesAsync();

            var output = new UsuarioOutput
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.Nome,
                Email = usuario.Email
            };

            return Ok(ApiResponse<UsuarioOutput>.Ok(output, "Usuário atualizado com sucesso."));
        }


        // DELETE - Remover usuário
        [HttpDelete("{id}", Name = "DeleteUsuario")]
        [SwaggerOperation(Summary = "Remove um usuário")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Usuário removido com sucesso")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Usuário não encontrado")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(ApiResponse<string>.Fail("Usuário não encontrado."));

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // HATEOAS Helpers
        private string GetByIdUrl(int id) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetUsuario), "Usuario", new { id }) ?? string.Empty;

        private string GetPageUrl(int page, int pageSize) =>
            _linkGenerator.GetUriByAction(HttpContext, nameof(GetUsuarios), "Usuario", new { page, pageSize }) ?? string.Empty;
    }
}
