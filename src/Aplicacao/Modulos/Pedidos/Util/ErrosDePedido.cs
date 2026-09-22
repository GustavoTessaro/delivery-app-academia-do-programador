using DeliveryApp.Aplicacao.Compartilhado;
using FluentResults;

namespace DeliveryApp.Aplicacao.Modulos.Pedidos.Util;

public sealed record ErroValidacaoPedido(string Campo, string Mensagem);

public static class ErrosDePedido
{
    public static Error NaoAutorizado()
    {
        return new Error("É necessário estar autenticado com o perfil adequado para acessar pedidos.")
            .WithMetadata(nameof(TipoErro), TipoErro.NaoAutorizado);
    }

    public static Error NaoEncontrado()
    {
        return new Error("Pedido não encontrado.")
            .WithMetadata(nameof(TipoErro), TipoErro.NaoEncontrado);
    }

    public static Error Conflito(string mensagem)
    {
        return new Error(mensagem)
            .WithMetadata(nameof(TipoErro), TipoErro.Conflito);
    }

    public static Error Validacao(string mensagem, string campo)
    {
        return new Error(mensagem)
            .WithMetadata(nameof(TipoErro), TipoErro.Validacao)
            .WithMetadata("Campo", campo);
    }

    public static IEnumerable<Error> Validacao(IEnumerable<ErroValidacaoPedido> erros)
    {
        return erros.Select(erro => Validacao(erro.Mensagem, erro.Campo));
    }
}
