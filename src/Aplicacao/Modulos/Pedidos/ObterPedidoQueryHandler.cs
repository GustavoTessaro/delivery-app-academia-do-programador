using DeliveryApp.Aplicacao.Modulos.Pedidos.DTOs;
using DeliveryApp.Aplicacao.Modulos.Pedidos.Util;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Pedidos;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Pedidos;

public sealed record ObterPedidoQuery(
    Guid PedidoId,
    TipoUsuario TipoUsuario
) : IRequest<Result<PedidoDto>>;


public sealed class ObterPedidoQueryHandler(
    IRepositorioPedido repositorioPedido,
    IProvedorDeUsuario provedorDeUsuario
) : IRequestHandler<ObterPedidoQuery, Result<PedidoDto>>
{
    public async Task<Result<PedidoDto>> Handle(
        ObterPedidoQuery request,
        CancellationToken cancellationToken
    )
    {
        if (provedorDeUsuario.Id is not Guid usuarioId)
            return Result.Fail<PedidoDto>(ErrosDePedido.NaoAutorizado());

        if (!provedorDeUsuario.PossuiTipo(request.TipoUsuario))
            return Result.Fail<PedidoDto>(ErrosDePedido.NaoAutorizado());

        if (request.PedidoId == Guid.Empty)
            return Result.Fail<PedidoDto>(ErrosDePedido.Validacao(
                "O pedido é obrigatório.", nameof(request.PedidoId)));

        Pedido? pedido = await repositorioPedido.ObterDoUsuarioAsync(
            request.PedidoId,
            usuarioId,
            request.TipoUsuario,
            cancellationToken
        );

        return pedido is null
            ? Result.Fail<PedidoDto>(ErrosDePedido.NaoEncontrado())
            : Result.Ok(PedidoDto.Criar(pedido));
    }
}
