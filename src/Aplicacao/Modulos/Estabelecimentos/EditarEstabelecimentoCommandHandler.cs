using DeliveryApp.Aplicacao.Modulos.Estabelecimentos.Util;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Estabelecimentos;

public sealed record EditarEstabelecimentoCommand(
    Guid EstabelecimentoId,
    string NomeComercial,
    string Documento,
    string Endereco,
    string Telefone,
    string AreaAtendimento,
    TimeOnly HorarioAbertura,
    TimeOnly HorarioFechamento
) : IRequest<Result>;

public sealed class EditarEstabelecimentoCommandHandler(
    IRepositorioEstabelecimento repositorioEstabelecimento,
    IProvedorDeUsuario provedorDeUsuario
) : IRequestHandler<EditarEstabelecimentoCommand, Result>
{
    public async Task<Result> Handle(
        EditarEstabelecimentoCommand command,
        CancellationToken cancellationToken = default
    )
    {
        if (provedorDeUsuario.Id != command.EstabelecimentoId)
            return Result.Fail(ErrosDeEstabelecimento.NaoAutorizado(command.EstabelecimentoId));

        var estabelecimentoAtualizado = new Estabelecimento(
            command.EstabelecimentoId,
            command.NomeComercial,
            command.Documento,
            command.Endereco,
            command.Telefone,
            command.AreaAtendimento,
            command.HorarioAbertura,
            command.HorarioFechamento
        );
        var erros = estabelecimentoAtualizado.Validar();

        if (erros.Count > 0)
            return Result.Fail(ErrosDeEstabelecimento.Validacao(erros));

        try
        {
            bool editado = await repositorioEstabelecimento.EditarAsync(
                command.EstabelecimentoId,
                estabelecimentoAtualizado,
                cancellationToken
            );

            return editado
                ? Result.Ok()
                : Result.Fail(ErrosDeEstabelecimento.NaoEncontrado(command.EstabelecimentoId));
        }
        catch (ConflitoDePersistenciaException)
        {
            return Result.Fail(ErrosDeEstabelecimento.ConflitoDePersistencia());
        }
    }
}
