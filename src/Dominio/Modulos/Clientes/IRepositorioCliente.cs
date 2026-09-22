using DeliveryApp.Dominio.Compartilhado;

namespace DeliveryApp.Dominio.Modulos.Clientes;

public interface IRepositorioCliente : IRepositorio<Cliente>
{
    Task<bool> ExisteRegistroComCpfAsync(
        string cpf,
        CancellationToken cancellationToken = default
    );
};
