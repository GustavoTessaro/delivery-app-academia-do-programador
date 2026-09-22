using DeliveryApp.Dominio.Compartilhado;

namespace DeliveryApp.Dominio.Modulos.Cardapio;

public sealed class Complemento : EntidadeBase<Complemento>
{
    public Guid ProdutoId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public decimal PrecoAdicional { get; private set; }

    private Complemento() { }

    public Complemento(Guid id, Guid produtoId, string nome, decimal precoAdicional)
    {
        Id = id;
        ProdutoId = produtoId;
        Nome = nome.Trim();
        PrecoAdicional = precoAdicional;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (Nome.Length is < 2 or > 100)
        {
            erros.Add(new(
                nameof(Nome),
                "O nome do complemento deve possuir entre 2 e 100 caracteres."
            ));
        }

        if (PrecoAdicional < 0)
        {
            erros.Add(new(
                nameof(PrecoAdicional),
                "O preço adicional do complemento não pode ser negativo."
            ));
        }

        if (PrecoAdicional != decimal.Round(PrecoAdicional, 2))
        {
            erros.Add(new(
                nameof(PrecoAdicional),
                "O preço adicional do complemento deve possuir no máximo duas casas decimais."
            ));
        }

        if (ProdutoId == Guid.Empty)
        {
            erros.Add(new(
                nameof(ProdutoId),
                "O produto do complemento é obrigatório."
            ));
        }

        return erros;
    }

    public override void Atualizar(Complemento entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome.Trim();
        PrecoAdicional = entidadeAtualizada.PrecoAdicional;
    }
}
