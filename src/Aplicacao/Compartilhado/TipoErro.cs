using FluentResults;

namespace DeliveryApp.Aplicacao.Compartilhado;

public enum TipoErro
{
    Validacao,
    NaoEncontrado,
    Conflito,
    NaoAutenticado,
    NaoAutorizado,
}

public static class TipoErroExtensions
{
    public static Error ObterMetadados(this TipoErro tipo, string campo, string mensagem)
    {
        return new Error(mensagem)
            .WithMetadata(nameof(TipoErro), tipo)
            .WithMetadata("Campo", campo);
    }
}
