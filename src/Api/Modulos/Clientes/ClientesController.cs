using DeliveryApp.Aplicacao.Modulos.Clientes;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.WebApi.Compartilhado.Http;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.WebApi.Modulos.Clientes;

[ApiController]
[Route("api/clientes")]
public sealed class ClientesController(
    IMediator mediator
) : ControllerBase
{

    [Authorize(Roles = nameof(TipoUsuario.Cliente))]
    [HttpGet("{clienteId:guid}")]
    [ProducesResponseType<ClienteResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ClienteResponse>> ObterPorId(
        Guid clienteId,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new ObterClientePorIdQuery(clienteId),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(new ClienteResponse(
            resultado.Value.Id,
            resultado.Value.Nome,
            resultado.Value.Cpf,
            resultado.Value.Email
        ));
    }

    [AllowAnonymous]
    [HttpPost("cadastro")]
    [ProducesResponseType<CadastrarClienteResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<CadastrarClienteResponse>> Cadastrar(
        CadastrarClienteRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(new CadastrarClienteCommand(
            request.Nome,
            request.Cpf,
            request.Email,
            request.Senha
        ), cancellationToken);

        if (!resultado.IsSuccess)
            return this.ProblemDetails(resultado);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { clienteId = resultado.Value },
            new CadastrarClienteResponse(
                resultado.Value,
                request.Nome
            )
        );
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AutenticarClienteResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AutenticarClienteResponse>> Autenticar(
        AutenticarClienteRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(new AutenticarClienteCommand(
            request.Email,
            request.Senha
        ), cancellationToken);

        if (!resultado.IsSuccess)
            return this.ProblemDetails(resultado);

        var accessTokenDoUsuario = resultado.Value;

        return Ok(new AutenticarClienteResponse(
            accessTokenDoUsuario.UsuarioId,
            accessTokenDoUsuario.Token,
            accessTokenDoUsuario.DataExpiracaoEmUtc
        ));
    }
}
