namespace DeliveryApp.Dominio.Compartilhado;

public interface IRepositorio<T> where T : EntidadeBase<T>
{
    Task CadastrarAsync(T entidade, CancellationToken cancellationToken = default);

    Task<bool> EditarAsync(
        Guid idSelecionado,
        T entidadeAtualizada,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExcluirAsync(Guid idSelecionado, CancellationToken cancellationToken = default);

    Task<T?> SelecionarPorIdAsync(
        Guid idSelecionado,
        CancellationToken cancellationToken = default
    );

    Task<List<T>> SelecionarTodosAsync(CancellationToken cancellationToken = default);
}
