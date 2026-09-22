using DeliveryApp.Aplicacao.Modulos.Estabelecimentos.DTOs;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Estabelecimentos;

public sealed record ListarEstabelecimentosDisponiveisQuery
    : IRequest<Result<IReadOnlyList<EstabelecimentoDto>>>;

public sealed class ListarEstabelecimentosDisponiveisQueryHandler(
    IRepositorioEstabelecimento repositorioEstabelecimento
) : IRequestHandler<
    ListarEstabelecimentosDisponiveisQuery,
    Result<IReadOnlyList<EstabelecimentoDto>>
>
{
    public async Task<Result<IReadOnlyList<EstabelecimentoDto>>> Handle(
        ListarEstabelecimentosDisponiveisQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var estabelecimentos = await repositorioEstabelecimento.SelecionarDisponiveisAsync(
            cancellationToken
        );

        return Result.Ok<IReadOnlyList<EstabelecimentoDto>>(
            estabelecimentos.Select(ParaDto).ToList()
        );
    }

    internal static EstabelecimentoDto ParaDto(Estabelecimento estabelecimento)
    {
        return new EstabelecimentoDto(
            estabelecimento.Id,
            estabelecimento.NomeComercial,
            estabelecimento.Documento,
            estabelecimento.Endereco,
            estabelecimento.Telefone,
            estabelecimento.AreaAtendimento,
            estabelecimento.HorarioAbertura,
            estabelecimento.HorarioFechamento,
            estabelecimento.Ativo
        );
    }
}
