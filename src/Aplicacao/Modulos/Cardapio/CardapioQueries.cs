using DeliveryApp.Aplicacao.Modulos.Cardapio.DTOs;
using DeliveryApp.Aplicacao.Modulos.Cardapio.Util;
using DeliveryApp.Dominio.Modulos.Cardapio;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Cardapio;

public sealed record ObterCardapioQuery(
    Guid EstabelecimentoId
) : IRequest<Result<CardapioDto>>;

public sealed class ObterCardapioQueryHandler(
    IRepositorioEstabelecimento repositorioEstabelecimento,
    IRepositorioProduto repositorioProduto
) : IRequestHandler<ObterCardapioQuery, Result<CardapioDto>>
{
    public async Task<Result<CardapioDto>> Handle(
        ObterCardapioQuery query,
        CancellationToken cancellationToken = default
    )
    {
        Estabelecimento? estabelecimento = await repositorioEstabelecimento.SelecionarPorIdAsync(
            query.EstabelecimentoId,
            cancellationToken
        );

        if (estabelecimento is null || !estabelecimento.Ativo)
            return Result.Fail(ErrosDeCardapio.CardapioIndisponivel(query.EstabelecimentoId));

        var produtos = await repositorioProduto.SelecionarCardapioAsync(
            query.EstabelecimentoId,
            cancellationToken
        );

        var categorias = produtos
            .Where(produto => produto.Categoria is not null)
            .GroupBy(produto => produto.Categoria!)
            .Select(grupo => new CategoriaDoCardapioDto(
                grupo.Key.Id,
                grupo.Key.Nome,
                grupo.Select(ListarProdutosQueryHandler.ParaDto).ToList()
            ))
            .OrderBy(categoria => categoria.Nome)
            .ToList();

        return Result.Ok(new CardapioDto(query.EstabelecimentoId, categorias));
    }
}
