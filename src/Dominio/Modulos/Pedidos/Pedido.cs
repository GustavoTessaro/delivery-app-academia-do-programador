using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;

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
    public List<TransicaoStatusPedido> Historico { get; private set; } = [];

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

        Historico = [new TransicaoStatusPedido(
            clienteId,
            TipoUsuario.Cliente,
            null,
            StatusPedido.AguardandoAceite,
            null,
            criadoEmUtc
        )];
    }

    public bool TentarObterNovoStatus(
        AcaoPedido acao,
        TipoUsuario tipoUsuario,
        out StatusPedido novoStatus,
        out string? erro
    )
    {
        switch (acao)
        {
            case AcaoPedido.Aceitar:
                novoStatus = StatusPedido.EmPreparo;
                break;

            case AcaoPedido.Recusar:
                novoStatus = StatusPedido.Recusado;
                break;

            case AcaoPedido.Cancelar:
                novoStatus = StatusPedido.Cancelado;
                break;

            case AcaoPedido.IniciarEntrega:
                novoStatus = StatusPedido.EmEntrega;
                break;

            case AcaoPedido.Concluir:
                novoStatus = StatusPedido.Concluido;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(acao));
        }

        bool transicaoPermitida;

        switch (Status, novoStatus)
        {
            case (StatusPedido.AguardandoAceite, StatusPedido.EmPreparo):
            case (StatusPedido.AguardandoAceite, StatusPedido.Recusado):
            case (StatusPedido.EmPreparo, StatusPedido.EmEntrega):
            case (StatusPedido.EmEntrega, StatusPedido.Concluido):
                transicaoPermitida = tipoUsuario == TipoUsuario.Estabelecimento;
                break;

            case (StatusPedido.AguardandoAceite, StatusPedido.Cancelado):
                transicaoPermitida = tipoUsuario == TipoUsuario.Cliente;
                break;

            default:
                transicaoPermitida = false;
                break;
        }

        if (!transicaoPermitida)
        {
            erro = $"A transição de {Status} para {novoStatus} não é permitida para {tipoUsuario}.";
            return false;
        }

        erro = null;
        return true;
    }

    public bool TentarAlterarStatus(
        AcaoPedido acao,
        Guid usuarioId,
        TipoUsuario tipoUsuario,
        string? motivo,
        DateTimeOffset ocorridaEmUtc,
        out string? erro
    )
    {
        if (motivo?.Trim().Length > TransicaoStatusPedido.TamanhoMaximoMotivo)
        {
            erro = $"O motivo deve possuir no máximo {TransicaoStatusPedido.TamanhoMaximoMotivo} caracteres.";
            return false;
        }

        if (!TentarObterNovoStatus(acao, tipoUsuario, out StatusPedido novoStatus, out erro))
            return false;

        StatusPedido statusAnterior = Status;
        Status = novoStatus;

        AtualizadoEmUtc = ocorridaEmUtc;
        Versao++;

        Historico.Add(new TransicaoStatusPedido(
            usuarioId,
            tipoUsuario,
            statusAnterior,
            Status,
            motivo,
            AtualizadoEmUtc
        ));

        erro = null;
        return true;
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
