using DeliveryApp.Aplicacao.Modulos.Cardapio.DTOs;
using DeliveryApp.Aplicacao.Modulos.Cardapio.Util;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Cardapio;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Cardapio;

public sealed record CadastrarCategoriaCommand(
    Guid EstabelecimentoId,
    string Nome
) : IRequest<Result<Guid>>;

public sealed class CadastrarCategoriaCommandHandler(
    IRepositorioCategoria repositorioCategoria,
    IProvedorDeUsuario provedorDeUsuario
) : IRequestHandler<CadastrarCategoriaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CadastrarCategoriaCommand command,
        CancellationToken cancellationToken = default
    )
    {
        if (provedorDeUsuario.Id != command.EstabelecimentoId)
            return Result.Fail(ErrosDeCardapio.NaoAutorizado(command.EstabelecimentoId));

        var categoria = new Categoria(
            Guid.CreateVersion7(),
            command.EstabelecimentoId,
            command.Nome
        );

        var erros = categoria.Validar();

        if (erros.Count > 0)
            return Result.Fail(ErrosDeCardapio.Validacao(erros));

        if (await repositorioCategoria.ExisteComNomeAsync(
                command.EstabelecimentoId,
                categoria.Nome,
                cancellationToken: cancellationToken
            ))
        {
            return Result.Fail(ErrosDeCardapio.CategoriaDuplicada(categoria.Nome));
        }

        try
        {
            await repositorioCategoria.CadastrarAsync(categoria, cancellationToken);
            return Result.Ok(categoria.Id);
        }
        catch (ConflitoDePersistenciaException)
        {
            return Result.Fail(ErrosDeCardapio.ConflitoDePersistencia());
        }
    }
}

public sealed record EditarCategoriaCommand(
    Guid EstabelecimentoId,
    Guid CategoriaId,
    string Nome
) : IRequest<Result>;

public sealed class EditarCategoriaCommandHandler(
    IRepositorioCategoria repositorioCategoria,
    IProvedorDeUsuario provedorDeUsuario
) : IRequestHandler<EditarCategoriaCommand, Result>
{
    public async Task<Result> Handle(
        EditarCategoriaCommand command,
        CancellationToken cancellationToken = default
    )
    {
        if (provedorDeUsuario.Id != command.EstabelecimentoId)
            return Result.Fail(ErrosDeCardapio.NaoAutorizado(command.EstabelecimentoId));

        var categoriaAtualizada = new Categoria(
            command.CategoriaId,
            command.EstabelecimentoId,
            command.Nome
        );

        var erros = categoriaAtualizada.Validar();

        if (erros.Count > 0)
            return Result.Fail(ErrosDeCardapio.Validacao(erros));

        if (await repositorioCategoria.ExisteComNomeAsync(
                command.EstabelecimentoId,
                categoriaAtualizada.Nome,
                command.CategoriaId,
                cancellationToken
            ))
        {
            return Result.Fail(ErrosDeCardapio.CategoriaDuplicada(categoriaAtualizada.Nome));
        }

        try
        {
            bool editado = await repositorioCategoria.EditarAsync(
                command.CategoriaId,
                command.EstabelecimentoId,
                categoriaAtualizada,
                cancellationToken
            );

            return editado
                ? Result.Ok()
                : Result.Fail(ErrosDeCardapio.NaoEncontrado("categoria", command.CategoriaId));
        }
        catch (ConflitoDePersistenciaException)
        {
            return Result.Fail(ErrosDeCardapio.ConflitoDePersistencia());
        }
    }
}

public sealed record ListarCategoriasQuery(
    Guid EstabelecimentoId
) : IRequest<Result<IReadOnlyList<CategoriaDto>>>;

public sealed class ListarCategoriasQueryHandler(
    IRepositorioCategoria repositorioCategoria,
    IProvedorDeUsuario provedorDeUsuario
) : IRequestHandler<ListarCategoriasQuery, Result<IReadOnlyList<CategoriaDto>>>
{
    public async Task<Result<IReadOnlyList<CategoriaDto>>> Handle(
        ListarCategoriasQuery query,
        CancellationToken cancellationToken = default
    )
    {
        if (provedorDeUsuario.Id != query.EstabelecimentoId)
            return Result.Fail(ErrosDeCardapio.NaoAutorizado(query.EstabelecimentoId));

        var categorias = await repositorioCategoria.SelecionarTodosAsync(
            query.EstabelecimentoId,
            cancellationToken
        );

        return Result.Ok<IReadOnlyList<CategoriaDto>>(
            categorias.Select(categoria => new CategoriaDto(
                categoria.Id,
                categoria.Nome
            )).ToList()
        );
    }
}
