namespace DeliveryApp.Aplicacao.Modulos.Cardapio.DTOs;

public sealed record ComplementoDto(
    Guid Id,
    string Nome,
    decimal PrecoAdicional
);

public sealed record ProdutoDto(
    Guid Id,
    Guid EstabelecimentoId,
    Guid CategoriaId,
    string CategoriaNome,
    string Nome,
    string Descricao,
    decimal Preco,
    bool Ativo,
    IReadOnlyList<ComplementoDto> Complementos
);

public sealed record CategoriaDto(Guid Id, string Nome);

public sealed record CategoriaDoCardapioDto(
    Guid Id,
    string Nome,
    IReadOnlyList<ProdutoDto> Produtos
);

public sealed record CardapioDto(
    Guid EstabelecimentoId,
    IReadOnlyList<CategoriaDoCardapioDto> Categorias
);
