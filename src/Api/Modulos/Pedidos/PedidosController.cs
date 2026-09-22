using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Aplicacao.Modulos.Pedidos;
using DeliveryApp.Aplicacao.Modulos.Pedidos.DTOs;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Pedidos;
using DeliveryApp.WebApi.Compartilhado.Http;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.WebApi.Modulos.Pedidos;

[ApiController]
[Route("api/pedidos")]
public sealed class PedidosController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = nameof(TipoUsuario.Cliente))]
    [HttpPost]
    [ProducesResponseType<CriarPedidoResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<CriarPedidoResponse>> Criar(
        CriarPedidoRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new CriarPedidoCommand(
                request.EstabelecimentoId,
                request.EnderecoEntrega,
                request.Itens.Select(item => new ItemCriarPedidoCommand(
                    item.ProdutoId,
                    item.Quantidade,
                    item.Observacao,
                    item.ComplementosIds
                )).ToList()
            ),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { pedidoId = resultado.Value },
            new CriarPedidoResponse(resultado.Value)
        );
    }

    [Authorize(Roles = nameof(TipoUsuario.Cliente) + "," + nameof(TipoUsuario.Estabelecimento))]
    [HttpGet("{pedidoId:guid}")]
    [ProducesResponseType<PedidoResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PedidoResponse>> ObterPorId(
        Guid pedidoId,
        CancellationToken cancellationToken
    )
    {
        var tipoUsuario = ObterTipoUsuarioAutenticado();
        var resultado = await mediator.Send(
            new ObterPedidoQuery(pedidoId, tipoUsuario),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(ParaResponse(resultado.Value));
    }

    [Authorize(Roles = nameof(TipoUsuario.Cliente) + "," + nameof(TipoUsuario.Estabelecimento))]
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<PedidoResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PedidoResponse>>> Listar(
        CancellationToken cancellationToken
    )
    {
        var tipoUsuario = ObterTipoUsuarioAutenticado();
        var resultado = await mediator.Send(
            new ListarPedidosQuery(tipoUsuario),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(resultado.Value.Select(ParaResponse).ToList());
    }

    private static PedidoResponse ParaResponse(PedidoDto pedido)
    {
        return new PedidoResponse(
            pedido.Id,
            pedido.ClienteId,
            pedido.EstabelecimentoId,
            pedido.EnderecoEntrega,
            pedido.Status.ToString(),
            pedido.Subtotal,
            pedido.TaxaEntrega,
            pedido.Total,
            pedido.CriadoEmUtc,
            pedido.AtualizadoEmUtc,
            pedido.Itens.Select(item => new ItemPedidoResponse(
                item.Id,
                item.ProdutoId,
                item.NomeProduto,
                item.Quantidade,
                item.PrecoUnitario,
                item.Observacao,
                item.ValorTotal,
                item.Complementos.Select(complemento => new ComplementoItemPedidoResponse(
                    complemento.Id,
                    complemento.ComplementoProdutoId,
                    complemento.Nome,
                    complemento.PrecoAdicional
                )).ToList()
            )).ToList()
        );
    }

    private TipoUsuario ObterTipoUsuarioAutenticado()
    {
        if (User.IsInRole(nameof(TipoUsuario.Cliente)))
            return TipoUsuario.Cliente;

        if (User.IsInRole(nameof(TipoUsuario.Estabelecimento)))
            return TipoUsuario.Estabelecimento;

        throw new UnauthorizedAccessException("Usuário sem perfil reconhecido.");
    }
}
