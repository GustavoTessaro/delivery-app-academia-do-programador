namespace DeliveryApp.Dominio.Modulos.Pedidos;

public sealed class ItemPedido
{
    public const int TamanhoMaximoNomeProduto = 100;
    public const int TamanhoMaximoObservacao = 500;

    public Guid Id { get; private set; }
    public Guid PedidoId { get; private set; }
    public Guid ProdutoId { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal ValorTotal { get; private set; }
    public string? Observacao { get; private set; }
    public List<ComplementoItemPedido> Complementos { get; set; } = [];

    private ItemPedido() { }

    public ItemPedido(
        Guid produtoId,
        string nomeProduto,
        int quantidade,
        decimal precoUnitario,
        string? observacao
    )
    {
        Id = Guid.CreateVersion7();
        ProdutoId = produtoId;
        NomeProduto = nomeProduto.Trim();
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
        ValorTotal = quantidade * (precoUnitario + Complementos.Sum(c => c.PrecoAdicional));
        Observacao = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim();
    }
}
