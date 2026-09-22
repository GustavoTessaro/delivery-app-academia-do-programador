using DeliveryApp.Aplicacao.Modulos.Pedidos.DTOs;
using DeliveryApp.Aplicacao.Modulos.Pedidos.Util;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Pedidos;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Pedidos;

public sealed record ListarPedidosQuery(TipoUsuario TipoUsuario)
    : IRequest<Result<IReadOnlyList<PedidoDto>>>;

public sealed class ListarPedidosQueryHandler(
    IRepositorioPedido repositorioPedido,
    IProvedorDeUsuario provedorDeUsuario
) : IRequestHandler<ListarPedidosQuery, Result<IReadOnlyList<PedidoDto>>>
{
    public async Task<Result<IReadOnlyList<PedidoDto>>> Handle(
        ListarPedidosQuery query,
        CancellationToken cancellationToken
    )
    {
        if (provedorDeUsuario.Id is not Guid usuarioId)
            return Result.Fail<IReadOnlyList<PedidoDto>>(ErrosDePedido.NaoAutorizado());

        if (!provedorDeUsuario.PossuiTipo(query.TipoUsuario))
            return Result.Fail<IReadOnlyList<PedidoDto>>(ErrosDePedido.NaoAutorizado());

        var registros = await repositorioPedido.ListarDoUsuarioAsync(
            usuarioId,
            query.TipoUsuario,
            cancellationToken
        );

        var dtos = registros.Select(PedidoDto.Criar).ToList();

        return Result.Ok<IReadOnlyList<PedidoDto>>(dtos);
    }
}
