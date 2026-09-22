using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Pedidos;
using DeliveryApp.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infraestrutura.Modulos.Pedidos;

public sealed class RepositorioPedidoEmOrm(DeliveryAppDbContext dbContext) : IRepositorioPedido
{
    public async Task CadastrarAsync(Pedido pedido, CancellationToken cancellationToken = default)
    {
        dbContext.Pedidos.Add(pedido);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Pedido>> ListarDoUsuarioAsync(
        Guid usuarioId,
        TipoUsuario tipoUsuario,
        CancellationToken cancellationToken = default
    )
    {
        return await FiltrarPorUsuario(ConsultaCompleta(), usuarioId, tipoUsuario)
            .OrderByDescending(p => p.CriadoEmUtc)
            .ThenByDescending(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Pedido?> ObterDoUsuarioAsync(
        Guid id,
        Guid usuarioId,
        TipoUsuario tipoUsuario,
        CancellationToken cancellationToken = default
    )
    {
        return await FiltrarPorUsuario(ConsultaCompleta(), usuarioId, tipoUsuario)
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Pedido?> ObterParaProcessamentoAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await ConsultaCompleta().SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task SalvarAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Pedido> ConsultaCompleta()
    {
        return dbContext.Pedidos
            .AsSplitQuery()
            .Include(p => p.Itens)
                .ThenInclude(i => i.Complementos);
    }

    private IQueryable<Pedido> FiltrarPorUsuario(
        IQueryable<Pedido> consulta,
        Guid usuarioId,
        TipoUsuario tipoUsuario
    )
    {
        if (tipoUsuario == TipoUsuario.Cliente)
            return consulta.Where(p => p.ClienteId == usuarioId);

        if (tipoUsuario == TipoUsuario.Estabelecimento)
        {
            return consulta.Where(p => dbContext.Estabelecimentos.Any(e =>
                e.Id == p.EstabelecimentoId &&
                e.Id == usuarioId
            ));
        }

        return consulta.Where(_ => false);
    }
}
