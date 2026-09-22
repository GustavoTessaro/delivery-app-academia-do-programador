using DeliveryApp.Dominio.Compartilhado;

namespace DeliveryApp.Dominio.Modulos.Cardapio;

public sealed class Categoria : EntidadeBase<Categoria>
{
    public Guid EstabelecimentoId { get; private set; }
    public string Nome { get; private set; } = string.Empty;

    private Categoria() { }

    public Categoria(Guid id, Guid estabelecimentoId, string nome)
    {
        Id = id;
        EstabelecimentoId = estabelecimentoId;
        Nome = nome.Trim();
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (Nome.Length is < 2 or > 100)
        {
            erros.Add(new(
                nameof(Nome),
                "O nome da categoria deve possuir entre 2 e 100 caracteres."
            ));
        }

        if (EstabelecimentoId == Guid.Empty)
        {
            erros.Add(new(
                nameof(EstabelecimentoId),
                "O estabelecimento da categoria é obrigatório."
            ));
        }

        return erros;
    }

    public override void Atualizar(Categoria entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome.Trim();
    }
}
