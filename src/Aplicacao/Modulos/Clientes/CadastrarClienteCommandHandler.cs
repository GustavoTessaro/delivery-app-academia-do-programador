using System.Data.Common;
using DeliveryApp.Aplicacao.Compartilhado;
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
        {
            var resultado = Result.Ok();

            foreach (ErroValidacao erro in erros)
                resultado.WithError(TipoErro.Validacao.ObterMetadados(erro.Campo, erro.Mensagem));

            return resultado;
        }

        var clientes = await repositorioCliente.SelecionarTodosAsync(cancellationToken);

        if (clientes.Any(registro => registro.Cpf == cliente.Cpf))
        {
            return Result.Fail(
                new Error("Um cliente com este CPF já foi cadastrado.")
                    .WithMetadata(nameof(TipoErro), TipoErro.Conflito)
            );
        }

        try
        {
            UsuarioCadastrado usuario = await gerenciadorDeIdentidade.CadastrarAsync(
                cliente.Id,
                command.Email,
                command.Senha,
                TipoUsuario.Cliente
            );

            await repositorioCliente.CadastrarAsync(cliente, cancellationToken);

            return Result.Ok(cliente.Id);
        }
        catch (ValidacaoDeIdentidadeException excecao)
        {
            return Result.Fail(ErrosDeCliente.ValidacaoDeIdentidade(excecao.Campo, excecao.Message));
        }
        catch (ConflitoDeIdentidadeException excecao)
        {
            return Result.Fail(ErrosDeCliente.ConflitoDeIdentidade(excecao.Message));
        }
        catch (DbException)
        {
            await gerenciadorDeIdentidade.ExcluirAsync(cliente.Id);

            return Result.Fail(ErrosDeCliente.CadastroDuplicado());
        }
    }
}
