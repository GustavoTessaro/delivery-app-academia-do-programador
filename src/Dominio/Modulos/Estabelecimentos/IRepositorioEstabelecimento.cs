using DeliveryApp.Dominio.Compartilhado;

namespace DeliveryApp.Dominio.Modulos.Estabelecimentos;

public interface IRepositorioEstabelecimento : IRepositorio<Estabelecimento>
{
    Task<List<Estabelecimento>> SelecionarDisponiveisAsync(
        CancellationToken cancellationToken = default
    );

    Task<bool> AlterarAtivoAsync(
        Guid estabelecimentoId,
        bool ativo,
        CancellationToken cancellationToken = default
    );
}
