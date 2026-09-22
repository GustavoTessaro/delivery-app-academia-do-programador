using DeliveryApp.Dominio.Compartilhado;

namespace DeliveryApp.Dominio.Modulos.Estabelecimentos;

public sealed class Estabelecimento : EntidadeBase<Estabelecimento>
{
    public string NomeComercial { get; private set; } = string.Empty;
    public string Documento { get; private set; } = string.Empty;
    public string Endereco { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;
    public string AreaAtendimento { get; private set; } = string.Empty;
    public TimeOnly HorarioAbertura { get; private set; }
    public TimeOnly HorarioFechamento { get; private set; }
    public bool Ativo { get; private set; }

    private Estabelecimento() { }

    public Estabelecimento(
        Guid id,
        string nomeComercial,
        string documento,
        string endereco,
        string telefone,
        string areaAtendimento,
        TimeOnly horarioAbertura,
        TimeOnly horarioFechamento
    )
    {
        Id = id;
        NomeComercial = nomeComercial.Trim();
        Documento = NormalizarStringNumerica(documento);
        Endereco = endereco.Trim();
        Telefone = NormalizarStringNumerica(telefone);
        AreaAtendimento = areaAtendimento.Trim();
        HorarioAbertura = horarioAbertura;
        HorarioFechamento = horarioFechamento;
        Ativo = true;
    }

    public void Ativar()
    {
        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (NomeComercial.Length is < 2 or > 100)
            erros.Add(new(nameof(NomeComercial), "O nome comercial deve possuir entre 2 e 100 caracteres."));

        if (Documento.Length is not 11 and not 14 || Documento.Any(c => !char.IsDigit(c)))
            erros.Add(new(nameof(Documento), "O documento deve possuir 11 ou 14 dígitos."));

        if (Endereco.Length is < 5 or > 250)
            erros.Add(new(nameof(Endereco), "O endereço deve possuir entre 5 e 250 caracteres."));

        if (Telefone.Length is < 10 or > 11 || Telefone.Any(c => !char.IsDigit(c)))
            erros.Add(new(nameof(Telefone), "O telefone deve possuir 10 ou 11 dígitos."));

        if (HorarioAbertura == HorarioFechamento)
            erros.Add(new(nameof(HorarioFechamento), "O horário de fechamento deve ser diferente do horário de abertura."));

        if (AreaAtendimento.Length is < 2 or > 150)
            erros.Add(new(nameof(AreaAtendimento), "A área de atendimento deve possuir entre 2 e 150 caracteres."));

        return erros;
    }

    public override void Atualizar(Estabelecimento entidadeAtualizada)
    {
        NomeComercial = entidadeAtualizada.NomeComercial;
        Documento = entidadeAtualizada.Documento;
        Endereco = entidadeAtualizada.Endereco;
        Telefone = entidadeAtualizada.Telefone;
        AreaAtendimento = entidadeAtualizada.AreaAtendimento;
        HorarioAbertura = entidadeAtualizada.HorarioAbertura;
        HorarioFechamento = entidadeAtualizada.HorarioFechamento;
    }

    private static string NormalizarStringNumerica(string valor)
    {
        return new string(valor.Where(char.IsDigit).ToArray());
    }
}
