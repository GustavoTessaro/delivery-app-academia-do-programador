using DeliveryApp.Dominio.Compartilhado;

namespace DeliveryApp.Dominio.Modulos.Cardapio;

public sealed class Produto : EntidadeBase<Produto>
{
    public Guid EstabelecimentoId { get; private set; }
    public Guid CategoriaId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public decimal Preco { get; private set; }
    public bool Ativo { get; private set; }

    public Categoria? Categoria { get; private set; }
    public List<Complemento> Complementos { get; private set; } = [];

    private Produto() { }

    public Produto(
        Guid id,
        Guid estabelecimentoId,
        Guid categoriaId,
        string nome,
        string descricao,
        decimal preco,
        IEnumerable<Complemento>? complementos = null
    )
    {
        Id = id;
        EstabelecimentoId = estabelecimentoId;
        CategoriaId = categoriaId;
        Nome = nome.Trim();
        Descricao = descricao.Trim();
        Preco = preco;
        Ativo = true;
        Complementos = complementos?.ToList() ?? [];
    }

    public void Ativar()
    {
        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
    }

    public void SubstituirComplementos(IEnumerable<Complemento> complementos)
    {
        Complementos.Clear();
        Complementos.AddRange(complementos);
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (EstabelecimentoId == Guid.Empty)
        {
            erros.Add(new(
                nameof(EstabelecimentoId),
                "O estabelecimento do produto é obrigatório."
            ));
        }

        if (CategoriaId == Guid.Empty)
        {
            erros.Add(new(
                nameof(CategoriaId),
                "A categoria do produto é obrigatória."
            ));
        }

        if (Nome.Length is < 2 or > 100)
        {
            erros.Add(new(
                nameof(Nome),
                "O nome do produto deve possuir entre 2 e 100 caracteres."
            ));
        }

        if (string.IsNullOrWhiteSpace(Descricao))
        {
            erros.Add(new(
                nameof(Descricao),
                "A descrição do produto é obrigatória."
            ));
        }

        if (Descricao.Length > 1000)
        {
            erros.Add(new(
                nameof(Descricao),
                "A descrição do produto deve possuir no máximo 1000 caracteres."
            ));
        }

        if (Preco <= 0)
        {
            erros.Add(new(
                nameof(Preco),
                "O preço do produto deve ser maior que zero."
            ));
        }

        if (Preco != decimal.Round(Preco, 2))
        {
            erros.Add(new(
                nameof(Preco),
                "O preço do produto deve possuir no máximo duas casas decimais."
            ));
        }

        foreach (Complemento complemento in Complementos)
        {
            erros.AddRange(complemento.Validar());
        }

        if (Complementos
            .GroupBy(complemento => complemento.Nome, StringComparer.OrdinalIgnoreCase)
            .Any(grupo => grupo.Count() > 1))
        {
            erros.Add(new(
                nameof(Complementos),
                "Não é permitido cadastrar complementos com o mesmo nome no produto."
            ));
        }

        return erros;
    }

    public override void Atualizar(Produto entidadeAtualizada)
    {
        CategoriaId = entidadeAtualizada.CategoriaId;
        Nome = entidadeAtualizada.Nome.Trim();
        Descricao = entidadeAtualizada.Descricao.Trim();
        Preco = entidadeAtualizada.Preco;
    }
}
