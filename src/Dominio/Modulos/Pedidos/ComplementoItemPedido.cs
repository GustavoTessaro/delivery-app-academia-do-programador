namespace DeliveryApp.Dominio.Modulos.Pedidos;

public sealed class ComplementoItemPedido
{
    public const int TamanhoMaximoNome = 100;

    public Guid Id { get; private set; }
    public Guid ItemPedidoId { get; private set; }
    public Guid ComplementoProdutoId { get; private set; }

    public string Nome { get; private set; } = string.Empty;
    public decimal PrecoAdicional { get; private set; }

    private ComplementoItemPedido() { }

    public ComplementoItemPedido(Guid complementoProdutoId, string nome, decimal precoAdicional)
    {
        Id = Guid.CreateVersion7();
        ComplementoProdutoId = complementoProdutoId;
        Nome = nome.Trim();
        PrecoAdicional = precoAdicional;
    }
}
