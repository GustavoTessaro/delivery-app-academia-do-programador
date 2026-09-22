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
}
