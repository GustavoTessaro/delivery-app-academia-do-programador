using DeliveryApp.Dominio.Modulos.Cardapio;
using DeliveryApp.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infraestrutura.Modulos.Cardapio;

public sealed class RepositorioProdutoEmOrm(
    DeliveryAppDbContext dbContext
) : RepositorioBaseEmOrm<Produto>(dbContext), IRepositorioProduto
{
    public Task<Produto?> SelecionarPorIdAsync(
        Guid produtoId,
        Guid estabelecimentoId,
        CancellationToken cancellationToken = default
    )
    {
        return registros
            .Include(produto => produto.Complementos)
            .SingleOrDefaultAsync(produto =>
                produto.Id == produtoId &&
                produto.EstabelecimentoId == estabelecimentoId,
                cancellationToken
            );
    }

    public Task<List<Produto>> SelecionarTodosAsync(
        Guid estabelecimentoId,
        CancellationToken cancellationToken = default
    )
    {
        return registros
            .Include(produto => produto.Categoria)
            .Include(produto => produto.Complementos)
            .Where(produto => produto.EstabelecimentoId == estabelecimentoId)
            .OrderBy(produto => produto.Categoria!.Nome)
            .ThenBy(produto => produto.Nome)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Produto>> SelecionarCardapioAsync(
        Guid estabelecimentoId,
        CancellationToken cancellationToken = default
    )
    {
        return registros
            .Include(produto => produto.Categoria)
            .Include(produto => produto.Complementos)
            .Where(produto =>
                produto.EstabelecimentoId == estabelecimentoId &&
                produto.Ativo)
            .OrderBy(produto => produto.Categoria!.Nome)
            .ThenBy(produto => produto.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> EditarAsync(
        Guid produtoId,
        Guid estabelecimentoId,
        Produto entidadeAtualizada,
        CancellationToken cancellationToken = default
    )
    {
        Produto? produto = await SelecionarPorIdAsync(
            produtoId,
            estabelecimentoId,
            cancellationToken
        );

        if (produto is null)
            return false;

        produto.Atualizar(entidadeAtualizada);
        produto.SubstituirComplementos(entidadeAtualizada.Complementos);

        await SalvarAlteracoesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> AlterarAtivoAsync(
        Guid produtoId,
        Guid estabelecimentoId,
        bool ativo,
        CancellationToken cancellationToken = default
    )
    {
        Produto? produto = await SelecionarPorIdAsync(
            produtoId,
            estabelecimentoId,
            cancellationToken
        );

        if (produto is null)
            return false;

        if (ativo)
            produto.Ativar();
        else
            produto.Desativar();

        await SalvarAlteracoesAsync(cancellationToken);

        return true;
    }
}
