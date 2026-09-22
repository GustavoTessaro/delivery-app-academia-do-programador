using DeliveryApp.Dominio.Compartilhado.Auth;

namespace DeliveryApp.Dominio.Modulos.Pedidos;

public interface IRepositorioPedido
{
    Task CadastrarAsync(Pedido pedido, CancellationToken cancellationToken = default);

    Task<Pedido?> ObterParaProcessamentoAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<Pedido?> ObterDoUsuarioAsync(
        Guid id,
        Guid usuarioId,
        TipoUsuario tipoUsuario,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<Pedido>> ListarDoUsuarioAsync(
        Guid usuarioId,
        TipoUsuario tipoUsuario,
        CancellationToken cancellationToken = default
    );

    Task SalvarAsync(CancellationToken cancellationToken = default);
}
