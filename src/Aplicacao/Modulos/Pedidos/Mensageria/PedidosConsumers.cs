using DeliveryApp.Dominio.Modulos.Cardapio;
using DeliveryApp.Dominio.Modulos.Clientes;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using DeliveryApp.Dominio.Modulos.Pedidos;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DeliveryApp.Aplicacao.Modulos.Pedidos.Mensageria;

public sealed class CriarPedidoConsumer(
    IRepositorioPedido repositorioPedido,
    IRepositorioCliente repositorioCliente,
    IRepositorioEstabelecimento repositorioEstabelecimento,
    IRepositorioProduto repositorioProduto,
    ILogger<CriarPedidoConsumer> logger
) : IConsumer<CriarPedidoMessage>
{
    public async Task Consume(ConsumeContext<CriarPedidoMessage> context)
    {
        var mensagem = context.Message;

        var pedidoParaProcessamento = await repositorioPedido.ObterParaProcessamentoAsync(mensagem.PedidoId, context.CancellationToken);

        if (pedidoParaProcessamento is not null)
        {
            logger.LogInformation("A criação do pedido {PedidoId} já foi processada.", mensagem.PedidoId);
            return;
        }

        var clienteExiste = await repositorioCliente.ExistePorIdAsync(
            mensagem.ClienteId,
            context.CancellationToken
        );

        if (!clienteExiste)
        {
            logger.LogInformation(
                "O cliente {ClienteId} não foi encontrado para o pedido {PedidoId}.",
                mensagem.ClienteId,
                mensagem.PedidoId
            );
            return;
        }

        var estabelecimento = await repositorioEstabelecimento.SelecionarParaPedidoAsync(
            mensagem.EstabelecimentoId,
            context.CancellationToken
        );

        var horaDeAgora = TimeOnly.FromTimeSpan(DateTimeOffset.UtcNow.TimeOfDay);

        if (estabelecimento is null || !estabelecimento.EstaDisponivel(horaDeAgora))
        {
            logger.LogInformation(
                "O estabelecimento {EstabelecimentoId} não está disponível para o pedido {PedidoId}.",
                mensagem.EstabelecimentoId,
                mensagem.PedidoId
            );
            return;
        }

        var produtosEncontrados = await repositorioProduto.ObterParaPedidoAsync(
            mensagem.EstabelecimentoId,
            mensagem.Itens.Select(i => i.ProdutoId),
            context.CancellationToken
        );

        Dictionary<Guid, Produto> produtosPorId = produtosEncontrados.ToDictionary(p => p.Id);

        if (produtosPorId.Count != mensagem.Itens.Select(i => i.ProdutoId).Distinct().Count())
        {
            logger.LogInformation(
                "O pedido {PedidoId} possui produtos indisponíveis.",
                mensagem.PedidoId
            );
            return;
        }

        List<ItemPedido> itens = [];

        foreach (ItemCriarPedidoMessage itemMensagem in mensagem.Itens)
        {
            var produto = produtosPorId[itemMensagem.ProdutoId];

            var complementos = produto.Complementos
                .Where(c => itemMensagem.ComplementosIds.Contains(c.Id))
                .ToList();

            if (complementos.Count != itemMensagem.ComplementosIds.Distinct().Count())
            {
                logger.LogInformation(
                    "O pedido {PedidoId} possui complementos indisponíveis.",
                    mensagem.PedidoId
                );
                return;
            }

            itens.Add(new ItemPedido(
                produto.Id,
                produto.Nome,
                (int)itemMensagem.Quantidade,
                produto.Preco,
                itemMensagem.Observacao,
                complementos.Select(c => new ComplementoItemPedido(
                    c.Id,
                    c.Nome,
                    c.PrecoAdicional
                ))
            ));
        }

        Pedido pedido = new(
             mensagem.PedidoId,
             mensagem.ClienteId,
             mensagem.EstabelecimentoId,
             mensagem.EnderecoEntrega,
             estabelecimento.TaxaEntrega,
             itens,
             mensagem.SolicitadoEmUtc
        );

        var erros = pedido.Validar().ToList();

        if (erros.Count > 0)
        {
            logger.LogInformation(
                "O pedido {PedidoId} é inválido: {Erros}.",
                mensagem.PedidoId,
                string.Join("; ", erros.Select(e => e.Mensagem))
            );
            return;
        }

        await repositorioPedido.CadastrarAsync(pedido, context.CancellationToken);
    }
}
