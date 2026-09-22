using DeliveryApp.Aplicacao.Modulos.Cardapio.DTOs;
using DeliveryApp.Aplicacao.Modulos.Cardapio.Util;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Cardapio;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Cardapio;

public sealed record ComplementoInput(string Nome, decimal PrecoAdicional);

public sealed record CadastrarProdutoCommand(
    Guid EstabelecimentoId,
    Guid CategoriaId,
    string Nome,
    string Descricao,
    decimal Preco,
    IReadOnlyCollection<ComplementoInput> Complementos
) : IRequest<Result<Guid>>;

public sealed class CadastrarProdutoCommandHandler(
    IRepositorioProduto repositorioProduto,
    IRepositorioCategoria repositorioCategoria,
    IProvedorDeUsuario provedorDeUsuario
) : IRequestHandler<CadastrarProdutoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CadastrarProdutoCommand command,
        CancellationToken cancellationToken = default
    )
    {
        if (provedorDeUsuario.Id != command.EstabelecimentoId)
            return Result.Fail(ErrosDeCardapio.NaoAutorizado(command.EstabelecimentoId));

        if (await repositorioCategoria.SelecionarPorIdAsync(
                command.CategoriaId,
                command.EstabelecimentoId,
                cancellationToken
            ) is null)
        {
            return Result.Fail(ErrosDeCardapio.NaoEncontrado("categoria", command.CategoriaId));
        }

        Guid produtoId = Guid.CreateVersion7();
        var produto = CriarProduto(command, produtoId);
        var erros = produto.Validar();

        if (erros.Count > 0)
            return Result.Fail(ErrosDeCardapio.Validacao(erros));

        try
        {
            await repositorioProduto.CadastrarAsync(produto, cancellationToken);
            return Result.Ok(produto.Id);
        }
        catch (ConflitoDePersistenciaException)
        {
            return Result.Fail(ErrosDeCardapio.ConflitoDePersistencia());
        }
    }

    internal static Produto CriarProduto(CadastrarProdutoCommand command, Guid produtoId)
    {
        return new Produto(
            produtoId,
            command.EstabelecimentoId,
            command.CategoriaId,
            command.Nome,
            command.Descricao,
            command.Preco,
            command.Complementos.Select(complemento => new Complemento(
                Guid.CreateVersion7(),
                produtoId,
                complemento.Nome,
                complemento.PrecoAdicional
            ))
        );
    }
}

public sealed record EditarProdutoCommand(
    Guid EstabelecimentoId,
    Guid ProdutoId,
    Guid CategoriaId,
    string Nome,
    string Descricao,
    decimal Preco,
    IReadOnlyCollection<ComplementoInput> Complementos
) : IRequest<Result>;

public sealed class EditarProdutoCommandHandler(
    IRepositorioProduto repositorioProduto,
    IRepositorioCategoria repositorioCategoria,
    IProvedorDeUsuario provedorDeUsuario
) : IRequestHandler<EditarProdutoCommand, Result>
{
    public async Task<Result> Handle(
        EditarProdutoCommand command,
        CancellationToken cancellationToken = default
    )
    {
        if (provedorDeUsuario.Id != command.EstabelecimentoId)
            return Result.Fail(ErrosDeCardapio.NaoAutorizado(command.EstabelecimentoId));

        if (await repositorioProduto.SelecionarPorIdAsync(
                command.ProdutoId,
                command.EstabelecimentoId,
                cancellationToken
            ) is null)
        {
            return Result.Fail(ErrosDeCardapio.NaoEncontrado("produto", command.ProdutoId));
        }

        if (await repositorioCategoria.SelecionarPorIdAsync(
                command.CategoriaId,
                command.EstabelecimentoId,
                cancellationToken
            ) is null)
        {
            return Result.Fail(ErrosDeCardapio.NaoEncontrado("categoria", command.CategoriaId));
        }

        var produtoAtualizado = CadastrarProdutoCommandHandler.CriarProduto(
            new CadastrarProdutoCommand(
                command.EstabelecimentoId,
                command.CategoriaId,
                command.Nome,
                command.Descricao,
                command.Preco,
                command.Complementos
            ),
            command.ProdutoId
        );
        var erros = produtoAtualizado.Validar();

        if (erros.Count > 0)
            return Result.Fail(ErrosDeCardapio.Validacao(erros));

        try
        {
            bool editado = await repositorioProduto.EditarAsync(
                command.ProdutoId,
                command.EstabelecimentoId,
                produtoAtualizado,
                cancellationToken
            );

            return editado
                ? Result.Ok()
                : Result.Fail(ErrosDeCardapio.NaoEncontrado("produto", command.ProdutoId));
        }
        catch (ConflitoDePersistenciaException)
        {
            return Result.Fail(ErrosDeCardapio.ConflitoDePersistencia());
        }
    }
}

public sealed record AlterarAtivoDoProdutoCommand(
    Guid EstabelecimentoId,
    Guid ProdutoId,
    bool Ativo
) : IRequest<Result>;

public sealed class AlterarAtivoDoProdutoCommandHandler(
    IRepositorioProduto repositorioProduto,
    IProvedorDeUsuario provedorDeUsuario
) : IRequestHandler<AlterarAtivoDoProdutoCommand, Result>
{
    public async Task<Result> Handle(
        AlterarAtivoDoProdutoCommand command,
        CancellationToken cancellationToken = default
    )
    {
        if (provedorDeUsuario.Id != command.EstabelecimentoId)
            return Result.Fail(ErrosDeCardapio.NaoAutorizado(command.EstabelecimentoId));

        try
        {
            bool alterado = await repositorioProduto.AlterarAtivoAsync(
                command.ProdutoId,
                command.EstabelecimentoId,
                command.Ativo,
                cancellationToken
            );

            return alterado
                ? Result.Ok()
                : Result.Fail(ErrosDeCardapio.NaoEncontrado("produto", command.ProdutoId));
        }
        catch (ConflitoDePersistenciaException)
        {
            return Result.Fail(ErrosDeCardapio.ConflitoDePersistencia());
        }
    }
}

public sealed record ListarProdutosQuery(
    Guid EstabelecimentoId
) : IRequest<Result<IReadOnlyList<ProdutoDto>>>;

public sealed class ListarProdutosQueryHandler(
    IRepositorioProduto repositorioProduto,
    IProvedorDeUsuario provedorDeUsuario
) : IRequestHandler<ListarProdutosQuery, Result<IReadOnlyList<ProdutoDto>>>
{
    public async Task<Result<IReadOnlyList<ProdutoDto>>> Handle(
        ListarProdutosQuery query,
        CancellationToken cancellationToken = default
    )
    {
        if (provedorDeUsuario.Id != query.EstabelecimentoId)
            return Result.Fail(ErrosDeCardapio.NaoAutorizado(query.EstabelecimentoId));

        var produtos = await repositorioProduto.SelecionarTodosAsync(
            query.EstabelecimentoId,
            cancellationToken
        );

        return Result.Ok<IReadOnlyList<ProdutoDto>>(produtos.Select(ParaDto).ToList());
    }

    internal static ProdutoDto ParaDto(Produto produto)
    {
        return new ProdutoDto(
            produto.Id,
            produto.EstabelecimentoId,
            produto.CategoriaId,
            produto.Categoria?.Nome ?? string.Empty,
            produto.Nome,
            produto.Descricao,
            produto.Preco,
            produto.Ativo,
            produto.Complementos.Select(complemento => new ComplementoDto(
                complemento.Id,
                complemento.Nome,
                complemento.PrecoAdicional
            )).ToList()
        );
    }
}
