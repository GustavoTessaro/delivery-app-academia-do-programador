using DeliveryApp.Dominio.Compartilhado;

namespace DeliveryApp.Dominio.Modulos.Pedidos;

public sealed class Pedido : EntidadeBase<Pedido>
{
    public const int TamanhoMinimoEndereco = 5;
    public const int TamanhoMaximoEndereco = 500;

    public Guid ClienteId { get; private set; }
    public Guid EstabelecimentoId { get; private set; }
    public string EnderecoEntrega { get; private set; } = string.Empty;
    public StatusPedido Status { get; private set; }

    public decimal Subtotal { get; private set; }
    public decimal TaxaEntrega { get; private set; }
    public decimal Total { get; private set; }

    public DateTimeOffset CriadoEmUtc { get; private set; }
    public DateTimeOffset AtualizadoEmUtc { get; private set; }
    public uint Versao { get; private set; }

    public List<ItemPedido> Itens { get; private set; } = [];

    public Pedido(
        Guid id,
        Guid clienteId,
        Guid estabelecimentoId,
        string enderecoEntrega,
        decimal taxaEntrega,
        IEnumerable<ItemPedido> itens,
        DateTimeOffset criadoEmUtc
    )
    {
        Id = id;
        ClienteId = clienteId;
        EstabelecimentoId = estabelecimentoId;
        EnderecoEntrega = enderecoEntrega.Trim();
        TaxaEntrega = taxaEntrega;
        Itens = itens.ToList();

        Subtotal = Itens.Sum(i => i.ValorTotal);
        Total = Subtotal + taxaEntrega;

        Status = StatusPedido.AguardandoAceite;
        CriadoEmUtc = criadoEmUtc;
        AtualizadoEmUtc = criadoEmUtc;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (ClienteId == Guid.Empty)
            erros.Add(new(nameof(ClienteId), "O cliente é obrigatório."));

        if (EstabelecimentoId == Guid.Empty)
            erros.Add(new(nameof(EstabelecimentoId), "O estabelecimento é obrigatório."));

        if (EnderecoEntrega.Length is < TamanhoMinimoEndereco or > TamanhoMaximoEndereco)
            erros.Add(new(nameof(EnderecoEntrega), $"O endereço deve possuir entre {TamanhoMinimoEndereco} e {TamanhoMaximoEndereco} caracteres."));

        if (Itens.Count == 0)
            erros.Add(new(nameof(Itens), "O pedido deve possuir ao menos um item."));

        if (TaxaEntrega < 0)
            erros.Add(new(nameof(TaxaEntrega), "A taxa de entrega não pode ser negativa."));

        for (int indice = 0; indice < Itens.Count; indice++)
        {
            ItemPedido item = Itens[indice];
            string campo = $"{nameof(Itens)}[{indice}]";

            if (item.ProdutoId == Guid.Empty)
                erros.Add(new($"{campo}.{nameof(item.ProdutoId)}", "O produto é obrigatório."));

            if (item.NomeProduto.Length is < 1 or > ItemPedido.TamanhoMaximoNomeProduto)
                erros.Add(new($"{campo}.{nameof(item.NomeProduto)}", $"O nome do produto deve possuir entre 1 e {ItemPedido.TamanhoMaximoNomeProduto} caracteres."));

            if (item.Quantidade <= 0)
                erros.Add(new($"{campo}.{nameof(item.Quantidade)}", "A quantidade deve ser maior que zero."));

            if (item.PrecoUnitario < 0)
                erros.Add(new($"{campo}.{nameof(item.PrecoUnitario)}", "O preço unitário não pode ser negativo."));

            if (item.Observacao?.Length > ItemPedido.TamanhoMaximoObservacao)
                erros.Add(new($"{campo}.{nameof(item.Observacao)}", $"A observação deve possuir no máximo {ItemPedido.TamanhoMaximoObservacao} caracteres."));

            foreach (ComplementoItemPedido complemento in item.Complementos)
            {
                if (complemento.ComplementoProdutoId == Guid.Empty)
                    erros.Add(new($"{campo}.{nameof(item.Complementos)}", "O complemento deve ser válido."));

                if (complemento.Nome.Length is < 1 or > ComplementoItemPedido.TamanhoMaximoNome)
                    erros.Add(new($"{campo}.{nameof(item.Complementos)}", $"O nome do complemento deve possuir entre 1 e {ComplementoItemPedido.TamanhoMaximoNome} caracteres."));

                if (complemento.PrecoAdicional < 0)
                    erros.Add(new($"{campo}.{nameof(item.Complementos)}", "O preço adicional não pode ser negativo."));
            }
        }

        return erros;
    }

    public override void Atualizar(Pedido entidadeAtualizada)
    {
        throw new NotSupportedException("Pedidos são alterados somento por transições de status.");
    }
}
