using DeliveryApp.Dominio.Modulos.Pedidos;

namespace DeliveryApp.Aplicacao.Modulos.Pedidos.DTOs;

public sealed record ComplementoItemPedidoDto(
    Guid Id,
    Guid ComplementoProdutoId,
    string Nome,
    decimal PrecoAdicional
);

public sealed record ItemPedidoDto(
    Guid Id,
    Guid ProdutoId,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario,
    string? Observacao,
    decimal ValorTotal,
    IReadOnlyList<ComplementoItemPedidoDto> Complementos
);

public sealed record PedidoDto(
    Guid Id,
    Guid ClienteId,
    Guid EstabelecimentoId,
    string EnderecoEntrega,
    StatusPedido Status,
    decimal Subtotal,
    decimal TaxaEntrega,
    decimal Total,
    DateTimeOffset CriadoEmUtc,
    DateTimeOffset AtualizadoEmUtc,
    IReadOnlyList<ItemPedidoDto> Itens
)
{
    public static PedidoDto Criar(Pedido pedido)
    {
        return new PedidoDto(
            pedido.Id,
            pedido.ClienteId,
            pedido.EstabelecimentoId,
            pedido.EnderecoEntrega,
            pedido.Status,
            pedido.Subtotal,
            pedido.TaxaEntrega,
            pedido.Total,
            pedido.CriadoEmUtc,
            pedido.AtualizadoEmUtc,

            pedido.Itens
                .OrderBy(i => i.Id)
                .Select(i => new ItemPedidoDto(
                    i.Id,
                    i.ProdutoId,
                    i.NomeProduto,
                    i.Quantidade,
                    i.PrecoUnitario,
                    i.Observacao,
                    i.ValorTotal,

                    i.Complementos
                        .OrderBy(c => c.Id)
                        .Select(c => new ComplementoItemPedidoDto(
                            c.Id,
                            c.ComplementoProdutoId,
                            c.Nome,
                            c.PrecoAdicional
                        ))
                        .ToList()
                )).ToList()
        );
    }
};
