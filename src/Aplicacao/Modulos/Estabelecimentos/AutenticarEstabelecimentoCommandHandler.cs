using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Aplicacao.Modulos.Estabelecimentos.Util;
using DeliveryApp.Dominio.Compartilhado.Auth;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Estabelecimentos;

public sealed class AutenticarEstabelecimentoCommandHandler(
    IGerenciadorDeIdentidade gerenciadorDeIdentidade,
    IEmissorDeTokens emissorDeTokens
) : IRequestHandler<AutenticarEstabelecimentoCommand, Result<AccessTokenDoUsuarioDto>>
{
    public async Task<Result<AccessTokenDoUsuarioDto>> Handle(
        AutenticarEstabelecimentoCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var usuario = await gerenciadorDeIdentidade.ChecarValidadeDeSenhaAsync(
            request.Email,
            request.Senha,
            TipoUsuario.Estabelecimento
        );

        if (usuario is null)
            return Result.Fail(ErrosDeEstabelecimento.CredenciaisInvalidas());

        var accessToken = emissorDeTokens.CriarToken(
            usuario.Id,
            usuario.Email,
            TipoUsuario.Estabelecimento
        );

        return Result.Ok(new AccessTokenDoUsuarioDto(
            usuario.Id,
            accessToken.Token,
            accessToken.DataExpiracaoEmUtc
        ));
    }
}
