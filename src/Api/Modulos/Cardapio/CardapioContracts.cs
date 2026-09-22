namespace DeliveryApp.WebApi.Modulos.Cardapio;

public sealed record CadastrarCategoriaRequest(string Nome);

public sealed record CategoriaResponse(Guid Id, string Nome);

public sealed record ComplementoRequest(string Nome, decimal PrecoAdicional);

public sealed record CadastrarProdutoRequest(
    Guid CategoriaId,
    string Nome,
    string Descricao,
    decimal Preco,
    IReadOnlyList<ComplementoRequest>? Complementos
);

public sealed record ComplementoResponse(
    Guid Id,
    string Nome,
    decimal PrecoAdicional
);

public sealed record ProdutoResponse(
    Guid Id,
    Guid EstabelecimentoId,
    Guid CategoriaId,
    string CategoriaNome,
    string Nome,
    string Descricao,
    decimal Preco,
    bool Ativo,
    IReadOnlyList<ComplementoResponse> Complementos
);

public sealed record CategoriaDoCardapioResponse(
    Guid Id,
    string Nome,
    IReadOnlyList<ProdutoResponse> Produtos
);

public sealed record CardapioResponse(
    Guid EstabelecimentoId,
    IReadOnlyList<CategoriaDoCardapioResponse> Categorias
);
