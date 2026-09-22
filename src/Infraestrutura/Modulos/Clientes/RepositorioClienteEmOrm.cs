using DeliveryApp.Dominio.Modulos.Clientes;
using DeliveryApp.Infraestrutura.Orm;

namespace DeliveryApp.Infraestrutura.Modulos.Clientes;

public sealed class RepositorioClienteEmOrm(
    DeliveryAppDbContext dbContext
) : RepositorioBaseEmOrm<Cliente>(dbContext), IRepositorioCliente
{
}
