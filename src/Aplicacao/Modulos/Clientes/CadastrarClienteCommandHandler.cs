using DeliveryApp.Aplicacao.Modulos.Clientes.Util;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Clientes;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Clientes;

public sealed record CadastrarClienteCommand(
    string Nome,
    string Cpf,
    string Email,
    string Senha
) : IRequest<Result<Guid>>;

public sealed class CadastrarClienteCommandHandler(
    IRepositorioCliente repositorioCliente,
    IGerenciadorDeIdentidade gerenciadorDeIdentidade
) : IRequestHandler<CadastrarClienteCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CadastrarClienteCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var cliente = new Cliente(
            Guid.CreateVersion7(),
            command.Nome,
            command.Cpf
        );

        var erros = cliente.Validar();

        if (erros.Count > 0)
            return Result.Fail(ErrosDeCliente.Validacao(erros));

        if (await repositorioCliente.ExisteRegistroComCpfAsync(cliente.Cpf, cancellationToken))
            return Result.Fail(ErrosDeCliente.CpfDuplicado());

        try
        {
            UsuarioDto usuario = await gerenciadorDeIdentidade.CadastrarAsync(
                cliente.Id,
                command.Email,
                command.Senha,
                TipoUsuario.Cliente
            );

            await repositorioCliente.CadastrarAsync(cliente, cancellationToken);

            return Result.Ok(cliente.Id);
        }
        catch (ValidacaoDeIdentidadeException ex)
        {
            return Result.Fail(ErrosDeCliente.ValidacaoDeIdentidade(ex.Campo, ex.Message));
        }
        catch (ConflitoDeIdentidadeException ex)
        {
            return Result.Fail(ErrosDeCliente.ConflitoDeIdentidade(ex.Message));
        }
        catch (ConflitoDePersistenciaException)
        {
            await gerenciadorDeIdentidade.ExcluirAsync(cliente.Id);

            return Result.Fail(ErrosDeCliente.CadastroDuplicado());
        }
    }
}
