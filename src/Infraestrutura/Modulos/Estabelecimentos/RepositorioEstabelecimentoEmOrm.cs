using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using DeliveryApp.Infraestrutura.Compartilhado.Orm;

namespace DeliveryApp.Infraestrutura.Modulos.Estabelecimentos;

public sealed class RepositorioEstabelecimentoEmOrm(
    DeliveryAppDbContext dbContext
) : RepositorioBaseEmOrm<Estabelecimento>(dbContext), IRepositorioEstabelecimento;
