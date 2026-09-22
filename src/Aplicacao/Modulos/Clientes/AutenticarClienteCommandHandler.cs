using DeliveryApp.Aplicacao.Modulos.Clientes.Util;
using DeliveryApp.Dominio.Compartilhado.Auth;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Clientes;

public sealed record AutenticarClienteCommand(
    string Email,
    string Senha
) : IRequest<Result<AccessTokenDoUsuarioDto>>;

public sealed record AccessTokenDoUsuarioDto(
    Guid UsuarioId,
    string Token,
    DateTime DataExpiracaoEmUtc
);

public sealed class AutenticarClienteCommandHandler(
    IGerenciadorDeIdentidade gerenciadorDeIdentidade,
    IEmissorDeTokens emissorDeTokens
) : IRequestHandler<AutenticarClienteCommand, Result<AccessTokenDoUsuarioDto>>
{
    public async Task<Result<AccessTokenDoUsuarioDto>> Handle(
        AutenticarClienteCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var usuario = await gerenciadorDeIdentidade.ChecarValidadeDeSenhaAsync(
            request.Email,
            request.Senha,
            TipoUsuario.Cliente
        );

        if (usuario is null)
            return Result.Fail(ErrosDeCliente.CredenciaisInvalidas());

        var accessToken = emissorDeTokens.CriarToken(
            usuario.Id,
            usuario.Email,
            TipoUsuario.Cliente
        );

        return Result.Ok(new AccessTokenDoUsuarioDto(
            usuario.Id,
            accessToken.Token,
            accessToken.DataExpiracaoEmUtc
        ));
    }
}
