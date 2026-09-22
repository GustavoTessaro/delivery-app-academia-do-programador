using DeliveryApp.Dominio.Compartilhado;

namespace DeliveryApp.Dominio.Modulos.Cardapio;

public interface IRepositorioProduto : IRepositorio<Produto>
{
    Task<Produto?> SelecionarPorIdAsync(
        Guid produtoId,
        Guid estabelecimentoId,
        CancellationToken cancellationToken = default
    );

    Task<List<Produto>> SelecionarTodosAsync(
        Guid estabelecimentoId,
        CancellationToken cancellationToken = default
    );

    Task<List<Produto>> SelecionarCardapioAsync(
        Guid estabelecimentoId,
        CancellationToken cancellationToken = default
    );

    Task<bool> EditarAsync(
        Guid produtoId,
        Guid estabelecimentoId,
        Produto entidadeAtualizada,
        CancellationToken cancellationToken = default
    );

    Task<bool> AlterarAtivoAsync(
        Guid produtoId,
        Guid estabelecimentoId,
        bool ativo,
        CancellationToken cancellationToken = default
    );
}
