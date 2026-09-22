using DeliveryApp.Dominio.Modulos.Pedidos;

namespace DeliveryApp.WebApi.Modulos.Pedidos;

public sealed record ItemPedidoRequest(
    Guid ProdutoId,
    int Quantidade,
    string? Observacao,
    IReadOnlyList<Guid> ComplementosIds
);

public sealed record CriarPedidoRequest(
    Guid EstabelecimentoId,
    string EnderecoEntrega,
    IReadOnlyList<ItemPedidoRequest> Itens
);

public sealed record CriarPedidoResponse(
    Guid Id
);

public sealed record MotivoPedidoRequest(
    string? Motivo
);

public sealed record AlterarStatusPedidoResponse(
    Guid PedidoId,
    AcaoPedido Acao
);

public sealed record ComplementoItemPedidoResponse(
    Guid Id,
    Guid ComplementoProdutoId,
    string Nome,
    decimal PrecoAdicional
);

public sealed record ItemPedidoResponse(
    Guid Id,
    Guid ProdutoId,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario,
    string? Observacao,
    decimal ValorTotal,
    IReadOnlyList<ComplementoItemPedidoResponse> Complementos
);

public sealed record PedidoResponse(
    Guid Id,
    Guid ClienteId,
    Guid EstabelecimentoId,
    string EnderecoEntrega,
    string Status,
    decimal Subtotal,
    decimal TaxaEntrega,
    decimal Total,
    DateTimeOffset CriadoEmUtc,
    DateTimeOffset AtualizadoEmUtc,
    IReadOnlyList<ItemPedidoResponse> Itens
);
