using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Aplicacao.Modulos.Estabelecimentos;
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
            string.Empty,
            new { clienteId = resultado.Value },
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
}
