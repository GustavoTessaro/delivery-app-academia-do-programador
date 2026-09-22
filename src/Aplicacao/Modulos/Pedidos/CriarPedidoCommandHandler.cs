using DeliveryApp.Aplicacao.Modulos.Pedidos.Util;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Pedidos;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Pedidos;

public sealed record ItemCriarPedidoCommand(
    Guid ProdutoId,
    int Quantidade,
    string? Observacao,
    IReadOnlyList<Guid> ComplementosIds
);

public sealed record CriarPedidoCommand(
    Guid EstabelecimentoId,
    string EnderecoEntrega,
    IReadOnlyList<ItemCriarPedidoCommand> Itens
) : IRequest<Result<Guid>>;

public sealed class CriarPedidoCommandHandler(
    IProvedorDeUsuario provedorDeUsuario
) : IRequestHandler<CriarPedidoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CriarPedidoCommand command,
        CancellationToken cancellationToken
    )
    {
        if (provedorDeUsuario.Id is not Guid clienteId)
            return Result.Fail<Guid>(ErrosDePedido.NaoAutorizado());

        if (!provedorDeUsuario.PossuiTipo(TipoUsuario.Cliente))
            return Result.Fail<Guid>(ErrosDePedido.NaoAutorizado());

        var erros = Validar(command);

        if (erros.Count > 0)
            return Result.Fail<Guid>(erros);

        var pedidoId = Guid.CreateVersion7();
        var solicitadoEmUtc = DateTimeOffset.UtcNow;

        // Criação e envio da CriarPedidoMessage

        return Result.Ok(pedidoId);
    }

    private static IReadOnlyList<Error> Validar(CriarPedidoCommand command)
    {
        List<ErroValidacaoPedido> erros = [];

        if (command.EstabelecimentoId == Guid.Empty)
            erros.Add(new(nameof(command.EstabelecimentoId), "O estabelecimento é obrigatório."));

        if (string.IsNullOrWhiteSpace(command.EnderecoEntrega) ||
            command.EnderecoEntrega.Trim().Length is < Pedido.TamanhoMinimoEndereco or > Pedido.TamanhoMaximoEndereco)
        {
            erros.Add(new(nameof(command.EnderecoEntrega), $"O endereço deve possuir entre {Pedido.TamanhoMinimoEndereco} e {Pedido.TamanhoMaximoEndereco} caracteres."));
        }

        if (command.Itens is null || command.Itens.Count == 0)
        {
            erros.Add(new(nameof(command.Itens), "O pedido deve possuir ao menos um item."));
        }
        else
        {
            for (int indice = 0; indice < command.Itens.Count; indice++)
            {
                ItemCriarPedidoCommand? item = command.Itens[indice];
                string campoItem = $"{nameof(command.Itens)}[{indice}]";

                if (item is null)
                {
                    erros.Add(new(campoItem, "O item do pedido é obrigatório."));
                    continue;
                }

                if (item.ProdutoId == Guid.Empty)
                    erros.Add(new($"{campoItem}.{nameof(item.ProdutoId)}", "O produto é obrigatório."));

                if (item.Quantidade <= 0)
                    erros.Add(new($"{campoItem}.{nameof(item.Quantidade)}", "A quantidade deve ser maior que zero."));

                if (item.Observacao?.Trim().Length > ItemPedido.TamanhoMaximoObservacao)
                {
                    erros.Add(new($"{campoItem}.{nameof(item.Observacao)}", $"A observação deve possuir no máximo {500} caracteres."));
                }

                if (item.ComplementosIds is null)
                {
                    erros.Add(new($"{campoItem}.{nameof(item.ComplementosIds)}", "A lista de complementos é obrigatória."));
                }
                else if (item.ComplementosIds.Any(id => id == Guid.Empty))
                {
                    erros.Add(new($"{campoItem}.{nameof(item.ComplementosIds)}", "Os complementos devem ser válidos."));
                }
            }
        }

        return ErrosDePedido.Validacao(erros).ToList();
    }
}
