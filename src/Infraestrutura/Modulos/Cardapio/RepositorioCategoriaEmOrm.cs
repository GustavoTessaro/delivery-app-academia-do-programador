using DeliveryApp.Dominio.Modulos.Cardapio;
using DeliveryApp.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infraestrutura.Modulos.Cardapio;

public sealed class RepositorioCategoriaEmOrm(
    DeliveryAppDbContext dbContext
) : RepositorioBaseEmOrm<Categoria>(dbContext), IRepositorioCategoria
{
    public async Task<bool> ExisteComNomeAsync(
        Guid estabelecimentoId,
        string nome,
        Guid? categoriaIdIgnorada = null,
        CancellationToken cancellationToken = default
    )
    {
        string nomeNormalizado = nome.Trim().ToLower();

        return await registros.AnyAsync(categoria =>
            categoria.EstabelecimentoId == estabelecimentoId &&
            categoria.Nome.ToLower() == nomeNormalizado &&
            (!categoriaIdIgnorada.HasValue || categoria.Id != categoriaIdIgnorada.Value),
            cancellationToken
        );
    }

    public Task<Categoria?> SelecionarPorIdAsync(
        Guid categoriaId,
        Guid estabelecimentoId,
        CancellationToken cancellationToken = default
    )
    {
        return registros.SingleOrDefaultAsync(categoria =>
            categoria.Id == categoriaId &&
            categoria.EstabelecimentoId == estabelecimentoId,
            cancellationToken
        );
    }

    public Task<List<Categoria>> SelecionarTodosAsync(
        Guid estabelecimentoId,
        CancellationToken cancellationToken = default
    )
    {
        return registros
            .Where(categoria => categoria.EstabelecimentoId == estabelecimentoId)
            .OrderBy(categoria => categoria.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> EditarAsync(
        Guid categoriaId,
        Guid estabelecimentoId,
        Categoria entidadeAtualizada,
        CancellationToken cancellationToken = default
    )
    {
        Categoria? categoria = await SelecionarPorIdAsync(
            categoriaId,
            estabelecimentoId,
            cancellationToken
        );

        if (categoria is null)
            return false;

        categoria.Atualizar(entidadeAtualizada);
        await SalvarAlteracoesAsync(cancellationToken);

        return true;
    }
}
