using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Models;
using JobFitScoreAPI.Services;
using Asp.Versioning;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/v{version:apiVersion}/proc")]
[ApiVersion("1.0")]
[Tags("procs")]
[Produces("application/json")]
[Consumes("application/json")]
public class PackageInsertController : ControllerBase
{
    private readonly OracleProcedureService _service;

    public PackageInsertController(OracleProcedureService service)
    {
        _service = service;
    }

    [HttpPost("usuario")]
    [AllowAnonymous]
    public ActionResult<long> InserirUsuario([FromQuery] string nome, [FromQuery] string email, [FromQuery] string senha)
    {
        return _service.InserirUsuario(nome, email, senha);
    }

    [HttpPost("empresa")]
    [AllowAnonymous]
    public ActionResult<long> InserirEmpresa([FromQuery] string nome, [FromQuery] string cnpj, [FromQuery] string email, [FromQuery] string senha)
    {
        return _service.InserirEmpresa(nome, cnpj, email, senha);
    }

    [HttpPost("vaga")]
    [AllowAnonymous]
    public ActionResult<long> InserirVaga([FromQuery] string titulo, [FromQuery] long empresaId)
    {
        return _service.InserirVaga(titulo, empresaId);
    }

    [HttpPost("vaga-habilidade")]
    [AllowAnonymous]
    public IActionResult InserirVagaHabilidade([FromQuery] long vagaId, [FromQuery] long habId)
    {
        _service.InserirVagaHabilidade(vagaId, habId);
        return Ok();
    }

    [HttpPost("habilidade")]
    [AllowAnonymous]
    public ActionResult<long> InserirHabilidade([FromQuery] string nome)
    {
        return _service.InserirHabilidade(nome);
    }

    [HttpPost("usuario-habilidade")]
    [AllowAnonymous]
    public IActionResult InserirUsuarioHabilidade([FromQuery] long usuarioId, [FromQuery] long habId)
    {
        _service.InserirUsuarioHabilidade(usuarioId, habId);
        return Ok();
    }

    [HttpPost("curso")]
    [AllowAnonymous]
    public ActionResult<long> InserirCurso([FromQuery] string nome,
                                           [FromQuery] string instituicao,
                                           [FromQuery] int cargaHoraria,
                                           [FromQuery] long usuarioId)
    {
        return _service.InserirCurso(nome, instituicao, cargaHoraria, usuarioId);
    }

    [HttpPost("candidatura")]
    [AllowAnonymous]
    public ActionResult<long> InserirCandidatura([FromQuery] long usuarioId, [FromQuery] long vagaId)
    {
        return _service.InserirCandidatura(usuarioId, vagaId);
    }
}
