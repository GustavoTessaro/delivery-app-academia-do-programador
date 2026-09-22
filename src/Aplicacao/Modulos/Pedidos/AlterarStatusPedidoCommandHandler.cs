using DeliveryApp.Aplicacao.Modulos.Pedidos.Mensageria;
using DeliveryApp.Aplicacao.Modulos.Pedidos.Util;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Pedidos;
using FluentResults;
using MassTransit;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Pedidos;

public sealed record AlterarStatusPedidoCommand(
    Guid PedidoId,
    TipoUsuario TipoUsuario,
    AcaoPedido Acao,
    string? Motivo
) : IRequest<Result<Guid>>;

public sealed class AlterarStatusPedidoCommandHandler(
    IRepositorioPedido repositorioPedido,
    IProvedorDeUsuario provedorDeUsuario,
    IPublishEndpoint publishEndpoint
) : IRequestHandler<AlterarStatusPedidoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        AlterarStatusPedidoCommand command,
        CancellationToken cancellationToken
    )
    {
        if (provedorDeUsuario.Id is not Guid usuarioId)
            return Result.Fail<Guid>(ErrosDePedido.NaoAutorizado());

        if (!provedorDeUsuario.PossuiTipo(command.TipoUsuario))
            return Result.Fail<Guid>(ErrosDePedido.NaoAutorizado());

        if (command.PedidoId == Guid.Empty)
            return Result.Fail<Guid>(ErrosDePedido.Validacao(
                "O pedido é obrigatório",
                nameof(command.PedidoId)
            ));

        if (command.Motivo?.Trim().Length > TransicaoStatusPedido.TamanhoMaximoMotivo)
        {
            return Result.Fail<Guid>(ErrosDePedido.Validacao(
                $"O motivo deve possuir no máximo {TransicaoStatusPedido.TamanhoMaximoMotivo} caracteres.",
                nameof(command.Motivo)
            ));
        }

        var pedido = await repositorioPedido.ObterParaProcessamentoAsync(
            command.PedidoId,
            cancellationToken
        );

        if (pedido is null)
            return Result.Fail<Guid>(ErrosDePedido.NaoEncontrado());

        var transicaoStatusValida = pedido.TentarObterNovoStatus(
            command.Acao,
            command.TipoUsuario,
            out _,
            out string? erro
        );

        if (!transicaoStatusValida)
            return Result.Fail<Guid>(ErrosDePedido.Conflito(erro!));

        await publishEndpoint.Publish(new AlterarStatusPedidoMessage(
            command.PedidoId,
            usuarioId,
            command.TipoUsuario,
            command.Acao,
            string.IsNullOrWhiteSpace(command.Motivo) ? null : command.Motivo.Trim(),
            DateTimeOffset.UtcNow,
            pedido.Versao
        ));

        return Result.Ok(command.PedidoId);
    }
}
