using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Services;
using JobFitScoreAPI.Dtos.Empresa;
using Swashbuckle.AspNetCore.Annotations;

namespace JobFitScoreAPI.Controllers.v1
{
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/empresas")]
[Tags("Empresas")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class EmpresaController : ControllerBase
{
private readonly AppDbContext _context;
private readonly LinkGenerator _linkGenerator;
private readonly ICryptoService _crypto;


    public EmpresaController(AppDbContext context, LinkGenerator linkGenerator, ICryptoService crypto)  
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

    // GET - Listar empresas  
    [HttpGet(Name = "GetEmpresas")]  
    [SwaggerOperation(Summary = "Lista todas as empresas", Description = "Retorna uma lista paginada de empresas.")]  
    [SwaggerResponse(StatusCodes.Status200OK, "Lista de empresas retornada com sucesso")]  
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno no servidor")]  
    public async Task<IActionResult> GetEmpresas(int page = 1, int pageSize = 10)  
    {  
        page = Math.Max(page, 1);  
        pageSize = Math.Max(pageSize, 1);  

        var totalItems = await _context.Empresas.CountAsync();  

        var empresas = await _context.Empresas  
            .OrderBy(e => e.NomeEmpresa)  
            .Skip((page - 1) * pageSize)  
            .Take(pageSize)  
            .Select(e => new EmpresaOutput  
            {  
                IdEmpresa = e.IdEmpresa,  
                Nome = e.NomeEmpresa,  
                Cnpj = e.Cnpj,  
                Email = e.Email  
            })  
            .ToListAsync();  

        var meta = new  
        {  
            totalItems,  
            page,  
            pageSize,  
            totalPages = Math.Ceiling((double)totalItems / pageSize)  
        };  

        return Ok(ApiResponse<object>.Ok(new { meta, data = empresas }, "Empresas listadas com sucesso."));  
    }  

    // GET - Buscar empresa por ID  
    [HttpGet("{id}", Name = "GetEmpresa")]  
    [SwaggerOperation(Summary = "Obtém uma empresa específica", Description = "Retorna os detalhes de uma empresa pelo ID.")]  
    [SwaggerResponse(StatusCodes.Status200OK, "Empresa encontrada com sucesso")]  
    [SwaggerResponse(StatusCodes.Status404NotFound, "Empresa não encontrada")]  
    public async Task<IActionResult> GetEmpresa(int id)  
    {  
        var empresa = await _context.Empresas  
            .Where(e => e.IdEmpresa == id)  
            .Select(e => new EmpresaOutput  
            {  
                IdEmpresa = e.IdEmpresa,  
                Nome = e.NomeEmpresa,  // CORRIGIDO  
                Cnpj = e.Cnpj,  
                Email = e.Email  
            })  
            .FirstOrDefaultAsync();  

        if (empresa == null)  
            return NotFound(ApiResponse<string>.Fail("Empresa não encontrada."));  

        var result = new  
        {  
            empresa,  
            links = new List<object>  
            {  
                new { rel = "self", href = GetByIdUrl(id), method = "GET" },  
                new { rel = "all", href = GetPageUrl(1, 10), method = "GET" }  
            }  
        };  

        return Ok(ApiResponse<object>.Ok(result, "Empresa encontrada com sucesso."));  
    }  

    // POST - Criar empresa  
    [AllowAnonymous]  
    [HttpPost(Name = "CreateEmpresa")]  
    [SwaggerOperation(Summary = "Cria uma nova empresa", Description = "Adiciona uma nova empresa no sistema.")]  
    [SwaggerResponse(StatusCodes.Status201Created, "Empresa criada com sucesso")]  
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro na requisição ou dados inválidos")]  
    public async Task<IActionResult> CreateEmpresa([FromBody] EmpresaInput input)  
    {  
        if (input == null)  
            return BadRequest(ApiResponse<string>.Fail("Input não pode ser nulo."));  

        if (string.IsNullOrWhiteSpace(input.Senha))
    return BadRequest(ApiResponse<string>.Fail("Senha é obrigatória."));

        var empresa = new Empresa
        {
            NomeEmpresa = input.Nome,
            Cnpj = input.Cnpj,
            Email = input.Email,
            Senha = _crypto.HashPassword(input.Senha)
        };

        _context.Empresas.Add(empresa);  
        await _context.SaveChangesAsync();  

        var output = new EmpresaOutput  
        {  
            IdEmpresa = empresa.IdEmpresa,  
            Nome = empresa.NomeEmpresa,  // CORRIGIDO  
            Cnpj = empresa.Cnpj,  
            Email = empresa.Email  
        };  

        return CreatedAtAction(nameof(GetEmpresa), new { id = empresa.IdEmpresa },  
            ApiResponse<EmpresaOutput>.Ok(output, "Empresa criada com sucesso."));  
    }  

    // PUT - Atualizar empresa  
    [HttpPut("{id}", Name = "UpdateEmpresa")]
    [SwaggerOperation(Summary = "Atualiza uma empresa existente", Description = "Modifica informações de uma empresa.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Empresa atualizada com sucesso")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro de validação ou dados inválidos")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Empresa não encontrada")]
    public async Task<IActionResult> UpdateEmpresa(int id, [FromBody] EmpresaUpdateInput input)
    {
        if (input == null)
            return BadRequest(ApiResponse<string>.Fail("Input não pode ser nulo."));

        var empresa = await _context.Empresas.FindAsync(id);
        if (empresa == null)
            return NotFound(ApiResponse<string>.Fail("Empresa não encontrada."));

        empresa.NomeEmpresa = input.Nome ?? empresa.NomeEmpresa;
        empresa.Email = input.Email ?? empresa.Email;

        if (!string.IsNullOrWhiteSpace(input.Senha))
            empresa.Senha = _crypto.HashPassword(input.Senha);

        await _context.SaveChangesAsync();

        var output = new EmpresaOutput
        {
            IdEmpresa = empresa.IdEmpresa,
            Nome = empresa.NomeEmpresa,
            Cnpj = empresa.Cnpj,
            Email = empresa.Email
        };

        return Ok(ApiResponse<EmpresaOutput>.Ok(output, "Empresa atualizada com sucesso."));
    }


    // DELETE - Remover empresa  
    [HttpDelete("{id}", Name = "DeleteEmpresa")]  
    [SwaggerOperation(Summary = "Remove uma empresa", Description = "Exclui uma empresa cadastrada do sistema.")]  
    [SwaggerResponse(StatusCodes.Status204NoContent, "Empresa removida com sucesso")]  
    [SwaggerResponse(StatusCodes.Status404NotFound, "Empresa não encontrada")]  
    public async Task<IActionResult> DeleteEmpresa(int id)  
    {  
        var empresa = await _context.Empresas.FindAsync(id);  
        if (empresa == null)  
            return NotFound(ApiResponse<string>.Fail("Empresa não encontrada."));  

        _context.Empresas.Remove(empresa);  
        await _context.SaveChangesAsync();  

        return NoContent();  
    }  

    // MÉTODOS AUXILIARES HATEOAS  
    private string GetByIdUrl(int id) =>  
        _linkGenerator.GetUriByAction(HttpContext, nameof(GetEmpresa), "Empresa", new { id }) ?? string.Empty;  

    private string GetPageUrl(int page, int pageSize) =>  
        _linkGenerator.GetUriByAction(HttpContext, nameof(GetEmpresas), "Empresa", new { page, pageSize }) ?? string.Empty;  
}  


}
