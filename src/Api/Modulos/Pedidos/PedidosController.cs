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
[Authorize(Roles = nameof(TipoUsuario.Cliente) + "," + nameof(TipoUsuario.Estabelecimento))]
public sealed class PedidosController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = nameof(TipoUsuario.Cliente))]
    [HttpPost]
    [ProducesResponseType<CriarPedidoResponse>(StatusCodes.Status202Accepted)]
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

        return AcceptedAtAction(
            nameof(ObterPorId),
            new { pedidoId = resultado.Value },
            new CriarPedidoResponse(resultado.Value)
        );
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpPatch("{pedidoId:guid}/aceite")]
    public async Task<ActionResult<AlterarStatusPedidoResponse>> Aceitar(
        Guid pedidoId,
        CancellationToken cancellationToken
    )
    {
        return await AlterarStatus(
            pedidoId,
            TipoUsuario.Estabelecimento,
            AcaoPedido.Aceitar,
            null,
            cancellationToken
        );
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpPatch("{pedidoId:guid}/recusa")]
    public async Task<ActionResult<AlterarStatusPedidoResponse>> Recusar(
        Guid pedidoId,
        MotivoPedidoRequest request,
        CancellationToken cancellationToken
    )
    {
        return await AlterarStatus(
            pedidoId,
            TipoUsuario.Estabelecimento,
            AcaoPedido.Recusar,
            request.Motivo,
            cancellationToken
        );
    }

    [Authorize(Roles = nameof(TipoUsuario.Cliente))]
    [HttpPatch("{pedidoId:guid}/cancelamento")]
    public async Task<ActionResult<AlterarStatusPedidoResponse>> Cancelar(
        Guid pedidoId,
        MotivoPedidoRequest request,
        CancellationToken cancellationToken
    )
    {
        return await AlterarStatus(
            pedidoId,
            TipoUsuario.Cliente,
            AcaoPedido.Cancelar,
            request.Motivo,
            cancellationToken
        );
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpPatch("{pedidoId:guid}/inicio-entrega")]
    public async Task<ActionResult<AlterarStatusPedidoResponse>> IniciarEntrega(
        Guid pedidoId,
        CancellationToken cancellationToken
    )
    {
        return await AlterarStatus(
            pedidoId,
            TipoUsuario.Estabelecimento,
            AcaoPedido.IniciarEntrega,
            null,
            cancellationToken
        );
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpPatch("{pedidoId:guid}/conclusao")]
    public async Task<ActionResult<AlterarStatusPedidoResponse>> Concluir(
        Guid pedidoId,
        CancellationToken cancellationToken
    )
    {
        return await AlterarStatus(
            pedidoId,
            TipoUsuario.Estabelecimento,
            AcaoPedido.Concluir,
            null,
            cancellationToken
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

    private async Task<ActionResult<AlterarStatusPedidoResponse>> AlterarStatus(
        Guid pedidoId,
        TipoUsuario tipoUsuario,
        AcaoPedido acao,
        string? motivo,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(new AlterarStatusPedidoCommand(
            pedidoId,
            tipoUsuario,
            acao,
            motivo
        ), cancellationToken);

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return AcceptedAtAction(
            nameof(ObterPorId),
            new { pedidoId = resultado.Value },
            new AlterarStatusPedidoResponse(resultado.Value, acao)
        );
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
