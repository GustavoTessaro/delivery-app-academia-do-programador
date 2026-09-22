using DeliveryApp.Dominio.Compartilhado;

namespace DeliveryApp.Dominio.Modulos.Clientes;

public sealed class Cliente : EntidadeBase<Cliente>
{
    public string Nome { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (Nome.Length is < 2 or > 100)
        {
            erros.Add(new ErroValidacao(
                nameof(Nome),
                "O nome deve possuir entre 2 e 100 caracteres."
            ));
        }

        if (Cpf.Length != 11 || Cpf.Any(c => !char.IsDigit(c)))
        {
            erros.Add(new ErroValidacao(
                nameof(Cpf),
                "O CPF deve possuir exatamente 11 dígitos."
            ));
        }

        return erros;
    }

    public override void Atualizar(Cliente entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome.Trim();
        Cpf = NormalizarCpf(entidadeAtualizada.Cpf);
    }

    private static string NormalizarCpf(string cpf) // 00023501232
    {
        return new string(cpf
            .Where(c => c is not '.' and not '-' && !char.IsWhiteSpace(c))
            .ToArray());
    }
}
