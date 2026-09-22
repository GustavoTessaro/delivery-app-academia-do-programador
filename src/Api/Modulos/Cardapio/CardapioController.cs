using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Aplicacao.Modulos.Cardapio;
using DeliveryApp.Aplicacao.Modulos.Cardapio.DTOs;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.WebApi.Compartilhado.Http;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.WebApi.Modulos.Cardapio;

[ApiController]
[Route("api/estabelecimentos/{estabelecimentoId:guid}")]
public sealed class CardapioController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("cardapio")]
    [ProducesResponseType<CardapioResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CardapioResponse>> ObterCardapio(
        Guid estabelecimentoId,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new ObterCardapioQuery(estabelecimentoId),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(ParaResponse(resultado.Value));
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpGet("categorias")]
    [ProducesResponseType<IReadOnlyList<CategoriaResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoriaResponse>>> ListarCategorias(
        Guid estabelecimentoId,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new ListarCategoriasQuery(estabelecimentoId),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(resultado.Value.Select(categoria => new CategoriaResponse(
            categoria.Id,
            categoria.Nome
        )).ToList());
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpPost("categorias")]
    [ProducesResponseType<CategoriaResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<CategoriaResponse>> CadastrarCategoria(
        Guid estabelecimentoId,
        CadastrarCategoriaRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new CadastrarCategoriaCommand(estabelecimentoId, request.Nome),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return CreatedAtAction(
            nameof(ListarCategorias),
            new { estabelecimentoId },
            new CategoriaResponse(resultado.Value, request.Nome.Trim())
        );
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpPut("categorias/{categoriaId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EditarCategoria(
        Guid estabelecimentoId,
        Guid categoriaId,
        CadastrarCategoriaRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new EditarCategoriaCommand(estabelecimentoId, categoriaId, request.Nome),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return NoContent();
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpGet("produtos")]
    [ProducesResponseType<IReadOnlyList<ProdutoResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProdutoResponse>>> ListarProdutos(
        Guid estabelecimentoId,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new ListarProdutosQuery(estabelecimentoId),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(resultado.Value.Select(ParaResponse).ToList());
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpPost("produtos")]
    [ProducesResponseType<ProdutoResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<ProdutoResponse>> CadastrarProduto(
        Guid estabelecimentoId,
        CadastrarProdutoRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new CadastrarProdutoCommand(
                estabelecimentoId,
                request.CategoriaId,
                request.Nome,
                request.Descricao,
                request.Preco,
                MapearComplementos(request.Complementos)
            ),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return CreatedAtAction(
            nameof(ListarProdutos),
            new { estabelecimentoId },
            new { id = resultado.Value }
        );
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpPut("produtos/{produtoId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EditarProduto(
        Guid estabelecimentoId,
        Guid produtoId,
        CadastrarProdutoRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new EditarProdutoCommand(
                estabelecimentoId,
                produtoId,
                request.CategoriaId,
                request.Nome,
                request.Descricao,
                request.Preco,
                MapearComplementos(request.Complementos)
            ),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return NoContent();
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpPatch("produtos/{produtoId:guid}/ativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> AtivarProduto(
        Guid estabelecimentoId,
        Guid produtoId,
        CancellationToken cancellationToken
    )
    {
        return AlterarAtivoProduto(estabelecimentoId, produtoId, true, cancellationToken);
    }

    [Authorize(Roles = nameof(TipoUsuario.Estabelecimento))]
    [HttpPatch("produtos/{produtoId:guid}/desativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> DesativarProduto(
        Guid estabelecimentoId,
        Guid produtoId,
        CancellationToken cancellationToken
    )
    {
        return AlterarAtivoProduto(estabelecimentoId, produtoId, false, cancellationToken);
    }

    private async Task<IActionResult> AlterarAtivoProduto(
        Guid estabelecimentoId,
        Guid produtoId,
        bool ativo,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new AlterarAtivoDoProdutoCommand(estabelecimentoId, produtoId, ativo),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return NoContent();
    }

    private static IReadOnlyCollection<ComplementoInput> MapearComplementos(
        IReadOnlyList<ComplementoRequest>? complementos
    )
    {
        return complementos?.Select(complemento => new ComplementoInput(
            complemento.Nome,
            complemento.PrecoAdicional
        )).ToList() ?? [];
    }

    private static ProdutoResponse ParaResponse(ProdutoDto produto)
    {
        return new ProdutoResponse(
            produto.Id,
            produto.EstabelecimentoId,
            produto.CategoriaId,
            produto.CategoriaNome,
            produto.Nome,
            produto.Descricao,
            produto.Preco,
            produto.Ativo,
            produto.Complementos.Select(complemento => new ComplementoResponse(
                complemento.Id,
                complemento.Nome,
                complemento.PrecoAdicional
            )).ToList()
        );
    }

    private static CardapioResponse ParaResponse(CardapioDto cardapio)
    {
        return new CardapioResponse(
            cardapio.EstabelecimentoId,
            cardapio.Categorias.Select(categoria => new CategoriaDoCardapioResponse(
                categoria.Id,
                categoria.Nome,
                categoria.Produtos.Select(ParaResponse).ToList()
            )).ToList()
        );
    }
}
