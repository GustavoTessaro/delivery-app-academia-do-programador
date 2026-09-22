using DeliveryApp.Aplicacao.Modulos.Estabelecimentos.Util;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Estabelecimentos;

public sealed record AlterarAtivoDoEstabelecimentoCommand(
    Guid EstabelecimentoId,
    bool Ativo
) : IRequest<Result>;

public sealed class AlterarAtivoDoEstabelecimentoCommandHandler(
    IRepositorioEstabelecimento repositorioEstabelecimento,
    IProvedorDeUsuario provedorDeUsuario
) : IRequestHandler<AlterarAtivoDoEstabelecimentoCommand, Result>
{
    public async Task<Result> Handle(
        AlterarAtivoDoEstabelecimentoCommand command,
        CancellationToken cancellationToken = default
    )
    {
        if (provedorDeUsuario.Id != command.EstabelecimentoId)
            return Result.Fail(ErrosDeEstabelecimento.NaoAutorizado(command.EstabelecimentoId));

        try
        {
            bool alterado = await repositorioEstabelecimento.AlterarAtivoAsync(
                command.EstabelecimentoId,
                command.Ativo,
                cancellationToken
            );

            return alterado
                ? Result.Ok()
                : Result.Fail(ErrosDeEstabelecimento.NaoEncontrado(command.EstabelecimentoId));
        }
        catch (ConflitoDePersistenciaException)
        {
            return Result.Fail(ErrosDeEstabelecimento.ConflitoDePersistencia());
        }
    }
}
