using DeliveryApp.Dominio.Modulos.Clientes;
using DeliveryApp.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infraestrutura.Modulos.Clientes;

public sealed class RepositorioClienteEmOrm(
    DeliveryAppDbContext dbContext
) : RepositorioBaseEmOrm<Cliente>(dbContext), IRepositorioCliente
{
    public async Task<bool> ExisteRegistroComCpfAsync(
        string cpf,
        CancellationToken cancellationToken = default
    )
    {
        return await registros.AnyAsync(r => r.Cpf == cpf, cancellationToken);
    }
}
