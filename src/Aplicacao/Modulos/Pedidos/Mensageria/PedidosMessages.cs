using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Pedidos;

namespace DeliveryApp.Aplicacao.Modulos.Pedidos.Mensageria;

public sealed record ItemCriarPedidoMessage(
    Guid ProdutoId,
    uint Quantidade,
    string? Observacao,
    IReadOnlyList<Guid> ComplementosIds
);

public sealed record CriarPedidoMessage(
    Guid PedidoId,
    Guid ClienteId,
    Guid EstabelecimentoId,
    string EnderecoEntrega,
    IReadOnlyList<ItemCriarPedidoMessage> Itens,
    DateTimeOffset SolicitadoEmUtc
);

public sealed record AlterarStatusPedidoMessage(
    Guid PedidoId,
    Guid UsuarioId,
    TipoUsuario TipoUsuario,
    AcaoPedido Acao,
    string? Motivo,
    DateTimeOffset SolicitadaEmUtc,
    uint VersaoEsperada
);
