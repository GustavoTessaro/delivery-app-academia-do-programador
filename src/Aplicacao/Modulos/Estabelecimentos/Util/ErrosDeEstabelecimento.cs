using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Dominio.Compartilhado;
using FluentResults;

namespace DeliveryApp.Aplicacao.Modulos.Estabelecimentos.Util;

public static class ErrosDeEstabelecimento
{
    public static Error CredenciaisInvalidas()
    {
        return new Error("O endereço de email ou senha informados são inválidos.")
            .WithMetadata(nameof(TipoErro), TipoErro.Validacao)
            .WithMetadata("Campo", "Credenciais");
    }

    public static Error CadastroDuplicado()
    {
        return new Error("Já existe um estabelecimento cadastrado com este nome comercial.")
            .WithMetadata(nameof(TipoErro), TipoErro.Conflito);
    }

    public static IEnumerable<Error> Validacao(IEnumerable<ErroValidacao> erros)
    {
        return erros.Select(erro => new Error(erro.Mensagem)
            .WithMetadata(nameof(TipoErro), TipoErro.Validacao)
            .WithMetadata("Campo", erro.Campo));
    }

    public static Error ConflitoDeIdentidade(string mensagem)
    {
        return new Error(mensagem)
            .WithMetadata(nameof(TipoErro), TipoErro.Conflito);
    }

    public static Error ValidacaoDeIdentidade(string campo, string mensagem)
    {
        return new Error(mensagem)
            .WithMetadata(nameof(TipoErro), TipoErro.Validacao)
            .WithMetadata("Campo", campo);
    }

    public static Error NaoEncontrado(Guid estabelecimentoId)
    {
        return new Error($"Não foi encontrado o estabelecimento {estabelecimentoId}.")
            .WithMetadata(nameof(TipoErro), TipoErro.NaoEncontrado);
    }

    public static Error NaoAutorizado(Guid estabelecimentoId)
    {
        return new Error(
                $"O usuário autenticado não pode administrar o estabelecimento {estabelecimentoId}."
            )
            .WithMetadata(nameof(TipoErro), TipoErro.NaoAutorizado);
    }

    public static Error ConflitoDePersistencia()
    {
        return new Error("Ocorreu um conflito ao persistir o estabelecimento.")
            .WithMetadata(nameof(TipoErro), TipoErro.Conflito);
    }
}
