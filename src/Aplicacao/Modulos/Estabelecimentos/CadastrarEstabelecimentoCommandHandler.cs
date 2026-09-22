using DeliveryApp.Aplicacao.Modulos.Estabelecimentos.Util;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using FluentResults;
using MediatR;

namespace DeliveryApp.Aplicacao.Modulos.Estabelecimentos;

public sealed record CadastrarEstabelecimentoCommand(
    string NomeComercial,
    string Documento,
    string Endereco,
    string Telefone,
    string AreaAtendimento,
    TimeOnly HorarioAbertura,
    TimeOnly HorarioFechamento,
    string Email,
    string Senha
) : IRequest<Result<Guid>>;

public sealed class CadastrarEstabelecimentoCommandHandler(
    IGerenciadorDeIdentidade gerenciadorDeIdentidade,
    IRepositorioEstabelecimento repositorioEstabelecimento
) : IRequestHandler<CadastrarEstabelecimentoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CadastrarEstabelecimentoCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var estabelecimento = new Estabelecimento(
            Guid.CreateVersion7(),
            command.NomeComercial,
            command.Documento,
            command.Endereco,
            command.Telefone,
            command.AreaAtendimento,
            command.HorarioAbertura,
            command.HorarioFechamento
        );

        var erros = estabelecimento.Validar();

        if (erros.Count > 0)
            return Result.Fail(ErrosDeEstabelecimento.Validacao(erros));

        try
        {
            UsuarioDto usuario = await gerenciadorDeIdentidade.CadastrarAsync(
                estabelecimento.Id,
                command.Email,
                command.Senha,
                TipoUsuario.Estabelecimento
            );

            await repositorioEstabelecimento.CadastrarAsync(estabelecimento, cancellationToken);

            return Result.Ok(estabelecimento.Id);
        }
        catch (ValidacaoDeIdentidadeException ex)
        {
            return Result.Fail(ErrosDeEstabelecimento.ValidacaoDeIdentidade(ex.Campo, ex.Message));
        }
        catch (ConflitoDeIdentidadeException ex)
        {
            return Result.Fail(ErrosDeEstabelecimento.ConflitoDeIdentidade(ex.Message));
        }
        catch (ConflitoDePersistenciaException)
        {
            await gerenciadorDeIdentidade.ExcluirAsync(estabelecimento.Id);

            return Result.Fail(ErrosDeEstabelecimento.CadastroDuplicado());
        }
    }
}
