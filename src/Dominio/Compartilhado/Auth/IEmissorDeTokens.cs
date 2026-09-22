namespace DeliveryApp.Dominio.Compartilhado.Auth;

public sealed record AccessToken(string Token, DateTime DataExpiracaoEmUtc);

public interface IEmissorDeTokens
{
    AccessToken CriarToken(Guid usuarioId, string email, TipoUsuario tipoUsuario);
}
