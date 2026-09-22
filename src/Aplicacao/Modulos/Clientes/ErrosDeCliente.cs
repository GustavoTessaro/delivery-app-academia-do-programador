using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Dominio.Compartilhado;
using FluentResults;

namespace DeliveryApp.Aplicacao.Modulos.Clientes;

public static class ErrosDeCliente
{
    public static Error CpfDuplicado()
    {
        return new Error("Já existe um cliente cadastrado com este CPF.")
            .WithMetadata(nameof(TipoErro), TipoErro.Conflito);
    }

    public static Error CadastroDuplicado()
    {
        return new Error("Já existe um cliente cadastrado com este email ou CPF.")
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
