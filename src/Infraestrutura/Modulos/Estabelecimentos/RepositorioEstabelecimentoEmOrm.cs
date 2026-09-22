using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using DeliveryApp.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infraestrutura.Modulos.Estabelecimentos;

public sealed class RepositorioEstabelecimentoEmOrm(
    DeliveryAppDbContext dbContext
) : RepositorioBaseEmOrm<Estabelecimento>(dbContext), IRepositorioEstabelecimento
{
    public Task<List<Estabelecimento>> SelecionarDisponiveisAsync(
        CancellationToken cancellationToken = default
    )
    {
        return registros
            .Where(estabelecimento => estabelecimento.Ativo)
            .OrderBy(estabelecimento => estabelecimento.NomeComercial)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AlterarAtivoAsync(
        Guid estabelecimentoId,
        bool ativo,
        CancellationToken cancellationToken = default
    )
    {
        Estabelecimento? estabelecimento = await SelecionarPorIdAsync(
            estabelecimentoId,
            cancellationToken
        );

        if (estabelecimento is null)
            return false;

        if (ativo)
            estabelecimento.Ativar();
        else
            estabelecimento.Desativar();

        await SalvarAlteracoesAsync(cancellationToken);

        return true;
    }
}
