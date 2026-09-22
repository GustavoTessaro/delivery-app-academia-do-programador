using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Modulos.Clientes;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Clientes;

public sealed record CadastrarClienteCommand(Guid Id, string Nome, string Cpf) : IRequest<Result>;

public sealed class CadastrarClienteCommandHandler(
    IRepositorioCliente repositorioCliente
) : IRequestHandler<CadastrarClienteCommand, Result>
{
    public async Task<Result> Handle(
        CadastrarClienteCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var cliente = new Cliente(
            command.Id,
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

        await repositorioCliente.CadastrarAsync(cliente, cancellationToken);

        return Result.Ok();
    }
}
