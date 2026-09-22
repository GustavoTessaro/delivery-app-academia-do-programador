using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DeliveryApp.Dominio.Compartilhado.Auth;

namespace DeliveryApp.Infraestrutura.Orm;

public sealed class DeliveryAppDbContext(
    DbContextOptions<DeliveryAppDbContext> options,
    IProvedorDeUsuario? provedorDeUsuario = null
) : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeliveryAppDbContext).Assembly);

        if (provedorDeUsuario is not null)
        {
        }
    }

    public override int SaveChanges()
    {
        Guid? usuarioId = provedorDeUsuario?.Id;

        if (!usuarioId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Não é possível salvar entidades do usuário sem estar autenticado."
            );
        }

        foreach (var entry in ChangeTracker.Entries<IEntidadeDeUsuario>())
        {
            Guid usuarioOriginalId = Guid.Empty;

            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.UsuarioId == Guid.Empty)
                    {
                        entry.Property(nameof(IEntidadeDeUsuario.UsuarioId)).CurrentValue = usuarioId.Value;
                    }
                    else if (entry.Entity.UsuarioId != usuarioId.Value)
                    {
                        throw new UnauthorizedAccessException(
                            "Tentativa de criar entidade para outro usuário."
                        );
                    }

                    break;

                case EntityState.Modified:
                    usuarioOriginalId = entry
                        .Property(nameof(IEntidadeDeUsuario.UsuarioId))
                        .OriginalValue is Guid idOriginal
                        ? idOriginal
                        : Guid.Empty;

                    Guid idAtualUsuario = entry
                        .Property(nameof(IEntidadeDeUsuario.UsuarioId))
                        .OriginalValue is Guid idAtual
                        ? idAtual
                        : Guid.Empty;

                    if (usuarioOriginalId != idAtualUsuario)
                    {
                        throw new UnauthorizedAccessException(
                              "Não é permitido alterar o usuário de uma entidade."
                          );
                    }

                    if (idAtualUsuario != usuarioId.Value)
                    {
                        throw new UnauthorizedAccessException(
                            "Tentativa de modificar entidade de outro usuário."
                        );
                    }

                    break;

                case EntityState.Deleted:
                    usuarioOriginalId = entry
                        .Property(nameof(IEntidadeDeUsuario.UsuarioId))
                        .OriginalValue is Guid original
                        ? original
                        : Guid.Empty;

                    if (usuarioOriginalId != usuarioId.Value)
                    {
                        throw new UnauthorizedAccessException(
                            "Tentativa de excluir entidade de outro usuário."
                        );
                    }

                    break;
            }
        }

        return base.SaveChanges();
    }
}
