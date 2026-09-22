using DeliveryApp.Dominio.Compartilhado;

namespace DeliveryApp.Dominio.Modulos.Cardapio;

public interface IRepositorioCategoria : IRepositorio<Categoria>
{
    Task<bool> ExisteComNomeAsync(
        Guid estabelecimentoId,
        string nome,
        Guid? categoriaIdIgnorada = null,
        CancellationToken cancellationToken = default
    );

    Task<Categoria?> SelecionarPorIdAsync(
        Guid categoriaId,
        Guid estabelecimentoId,
        CancellationToken cancellationToken = default
    );

    Task<List<Categoria>> SelecionarTodosAsync(
        Guid estabelecimentoId,
        CancellationToken cancellationToken = default
    );

    Task<bool> EditarAsync(
        Guid categoriaId,
        Guid estabelecimentoId,
        Categoria entidadeAtualizada,
        CancellationToken cancellationToken = default
    );
}
