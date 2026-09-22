using DeliveryApp.Dominio.Compartilhado.Auth;
using Microsoft.AspNetCore.Identity;

namespace DeliveryApp.Infraestrutura.Compartilhado.Auth;

public sealed class GerenciadorDeIdentidade(
    UserManager<IdentityUser<Guid>> userManager
) : IGerenciadorDeIdentidade
{
    public async Task<UsuarioDto> CadastrarAsync(
        Guid usuarioId,
        string email,
        string senha,
        TipoUsuario tipo
    )
    {
        var usuario = new IdentityUser<Guid>
        {
            Id = usuarioId,
            Email = email.Trim(),
            UserName = email.Trim()
        };

        var resultadoUsuario = await userManager.CreateAsync(usuario, senha);

        if (!resultadoUsuario.Succeeded)
            throw CriarErro(resultadoUsuario);

        var resultadoPapel = await userManager.AddToRoleAsync(usuario, tipo.ToString());

        if (!resultadoPapel.Succeeded)
        {
            await userManager.DeleteAsync(usuario);

            throw CriarErro(resultadoPapel);
        }

        return new UsuarioDto(usuario.Id, usuario.Email);
    }

    public async Task<UsuarioDto?> ChecarValidadeDeSenhaAsync(
        string email,
        string senha,
        TipoUsuario tipo
    )
    {
        var usuario = await userManager.FindByEmailAsync(email);

        if (usuario is null || await userManager.IsLockedOutAsync(usuario))
            return null;

        if (!await userManager.CheckPasswordAsync(usuario, senha))
        {
            await userManager.AccessFailedAsync(usuario);

            return null;
        }

        if (!await userManager.IsInRoleAsync(usuario, tipo.ToString()))
            return null;

        if (usuario.AccessFailedCount > 0)
            await userManager.ResetAccessFailedCountAsync(usuario);

        return new UsuarioDto(usuario.Id, usuario.Email!);
    }

    public async Task ExcluirAsync(Guid usuarioId)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString());

        if (usuario is not null)
            await userManager.DeleteAsync(usuario);
    }

    private static Exception CriarErro(IdentityResult resultado)
    {
        if (resultado.Errors.Any(
                     erro => erro.Code is "DuplicateEmail" or "DuplicateUserName"))
        {
            return new ConflitoDeIdentidadeException(
                "Já existe um usuário cadastrado com este email."
            );
        }

        var erro = resultado.Errors.First();

        string campo = erro.Code.StartsWith("Password", StringComparison.Ordinal)
            ? "Senha"
            : "Email";

        return new ValidacaoDeIdentidadeException(campo, erro.Description);
    }
}
