using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Aplicacao.Modulos.Estabelecimentos;
using DeliveryApp.Aplicacao.Modulos.Estabelecimentos.DTOs;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.WebApi.Compartilhado.Http;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.WebApi.Modulos.Estabelecimentos;

[ApiController]
[Route("api/estabelecimentos")]
public sealed class EstabelecimentosController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("cadastro")]
    [ProducesResponseType<CadastrarEstabelecimentoResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<CadastrarEstabelecimentoResponse>> Cadastrar(
       CadastrarEstabelecimentoRequest request,
       CancellationToken cancellationToken
   )
    {
        var resultado = await mediator.Send(new CadastrarEstabelecimentoCommand(
            request.NomeComercial,
            request.Documento,
            request.Endereco,
            request.Telefone,
            request.AreaAtendimento,
            request.HorarioAbertura,
            request.HorarioFechamento,
            request.Email,
            request.Senha
        ), cancellationToken);

        if (!resultado.IsSuccess)
            return this.ProblemDetails(resultado);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { estabelecimentoId = resultado.Value },
            new CadastrarEstabelecimentoResponse(
                resultado.Value,
                request.NomeComercial
            )
        );
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AutenticarEstabelecimentoResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AutenticarEstabelecimentoResponse>> Autenticar(
        AutenticarEstabelecimentoRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(new AutenticarEstabelecimentoCommand(
            request.Email,
            request.Senha
        ), cancellationToken);

        if (!resultado.IsSuccess)
            return this.ProblemDetails(resultado);

        var accessTokenDoUsuario = resultado.Value;

        return Ok(new AutenticarEstabelecimentoResponse(
            accessTokenDoUsuario.UsuarioId,
            accessTokenDoUsuario.Token,
            accessTokenDoUsuario.DataExpiracaoEmUtc
        ));
    }

    [Authorize(Roles = nameof(TipoUsuario.Cliente) + "," + nameof(TipoUsuario.Estabelecimento))]
    [HttpGet("disponiveis")]
    [ProducesResponseType<IReadOnlyList<EstabelecimentoResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EstabelecimentoResponse>>> ListarDisponiveis(
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new ListarEstabelecimentosDisponiveisQuery(),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(resultado.Value.Select(ParaResponse).ToList());
    }

    [Authorize(Roles = nameof(TipoUsuario.Cliente) + "," + nameof(TipoUsuario.Estabelecimento))]
    [HttpGet("{estabelecimentoId:guid}")]
    [ProducesResponseType<EstabelecimentoResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<EstabelecimentoResponse>> ObterPorId(
        Guid estabelecimentoId,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new ObterEstabelecimentoPorIdQuery(estabelecimentoId),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(ParaResponse(resultado.Value));
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpPut("{estabelecimentoId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Editar(
        Guid estabelecimentoId,
        EditarEstabelecimentoRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(new EditarEstabelecimentoCommand(
            estabelecimentoId,
            request.NomeComercial,
            request.Documento,
            request.Endereco,
            request.Telefone,
            request.AreaAtendimento,
            request.HorarioAbertura,
            request.HorarioFechamento
        ), cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return NoContent();
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpPatch("{estabelecimentoId:guid}/ativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> Ativar(
        Guid estabelecimentoId,
        CancellationToken cancellationToken
    )
    {
        return AlterarAtivo(estabelecimentoId, true, cancellationToken);
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpPatch("{estabelecimentoId:guid}/desativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> Desativar(
        Guid estabelecimentoId,
        CancellationToken cancellationToken
    )
    {
        return AlterarAtivo(estabelecimentoId, false, cancellationToken);
    }

    private async Task<IActionResult> AlterarAtivo(
        Guid estabelecimentoId,
        bool ativo,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new AlterarAtivoDoEstabelecimentoCommand(estabelecimentoId, ativo),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return NoContent();
    }

    private static EstabelecimentoResponse ParaResponse(EstabelecimentoDto estabelecimento)
    {
        return new EstabelecimentoResponse(
            estabelecimento.Id,
            estabelecimento.NomeComercial,
            estabelecimento.Documento,
            estabelecimento.Endereco,
            estabelecimento.Telefone,
            estabelecimento.AreaAtendimento,
            estabelecimento.HorarioAbertura,
            estabelecimento.HorarioFechamento,
            estabelecimento.Ativo
        );
    }
}
