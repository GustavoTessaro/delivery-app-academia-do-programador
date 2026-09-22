using DeliveryApp.Dominio.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infraestrutura.Orm;

public abstract class RepositorioBaseEmOrm<T>(DeliveryAppDbContext dbContext) where T : EntidadeBase<T>
{
    protected readonly DbSet<T> registros = dbContext.Set<T>();

    public async Task CadastrarAsync(T entidade, CancellationToken cancellationToken = default)
    {
        registros.Add(entidade);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> EditarAsync(
        Guid id,
        T entidadeAtualizada,
        CancellationToken cancellationToken = default
    )
    {
        T? registroSelecionado = await SelecionarPorIdAsync(id, cancellationToken);

        if (registroSelecionado == null)
            return false;

        registroSelecionado.Atualizar(entidadeAtualizada);

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        T? TSelecionado = await SelecionarPorIdAsync(id, cancellationToken);

        if (TSelecionado == null)
            return false;

        registros.Remove(TSelecionado);

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public virtual async Task<T?> SelecionarPorIdAsync(Guid idSelecionado, CancellationToken cancellationToken = default)
    {
        return await registros.SingleOrDefaultAsync(c => c.Id == idSelecionado);
    }

    public virtual async Task<List<T>> SelecionarTodosAsync(CancellationToken cancellationToken = default)
    {
        return await registros.ToListAsync();
    }
}
