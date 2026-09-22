namespace DeliveryApp.Dominio.Compartilhado.Auth;

public sealed class ValidacaoDeIdentidadeException(
    string campo,
    string mensagem
) : Exception(mensagem)
{
    public string Campo { get; } = campo;
}

public sealed class ConflitoDeIdentidadeException(string mensagem) : Exception(mensagem);

public sealed record UsuarioDto(Guid Id, string Email);

public interface IGerenciadorDeIdentidade
{
    Task<UsuarioDto> CadastrarAsync(
        Guid usuarioId,
        string email,
        string senha,
        TipoUsuario tipo
    );
    Task<UsuarioDto?> ChecarValidadeDeSenhaAsync(
        string email,
        string senha,
        TipoUsuario tipo
    );
    Task ExcluirAsync(Guid usuarioId);
}
