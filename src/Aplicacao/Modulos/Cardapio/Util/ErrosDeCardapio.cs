using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Dominio.Compartilhado;
using FluentResults;

namespace DeliveryApp.Aplicacao.Modulos.Cardapio.Util;

public static class ErrosDeCardapio
{
    public static IEnumerable<Error> Validacao(IEnumerable<ErroValidacao> erros)
    {
        return erros.Select(erro => new Error(erro.Mensagem)
            .WithMetadata(nameof(TipoErro), TipoErro.Validacao)
            .WithMetadata("Campo", erro.Campo));
    }

    public static Error NaoAutorizado(Guid estabelecimentoId)
    {
        return new Error(
                $"O usuário autenticado não pode administrar o estabelecimento {estabelecimentoId}."
            )
            .WithMetadata(nameof(TipoErro), TipoErro.NaoAutorizado);
    }

    public static Error NaoEncontrado(string recurso, Guid id)
    {
        return new Error($"Não foi encontrado o {recurso} {id}.")
            .WithMetadata(nameof(TipoErro), TipoErro.NaoEncontrado);
    }

    public static Error CardapioIndisponivel(Guid estabelecimentoId)
    {
        return new Error(
                $"O cardápio do estabelecimento {estabelecimentoId} não está disponível."
            )
            .WithMetadata(nameof(TipoErro), TipoErro.NaoEncontrado);
    }

    public static Error CategoriaDuplicada(string nome)
    {
        return new Error($"Já existe uma categoria chamada \"{nome}\" neste estabelecimento.")
            .WithMetadata(nameof(TipoErro), TipoErro.Conflito);
    }

    public static Error ConflitoDePersistencia()
    {
        return new Error("Ocorreu um conflito ao persistir o cardápio.")
            .WithMetadata(nameof(TipoErro), TipoErro.Conflito);
    }
}
