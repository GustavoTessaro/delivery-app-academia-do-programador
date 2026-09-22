namespace DeliveryApp.Dominio.Compartilhado;

public sealed record ErroValidacao(string Campo, string Mensagem);

public abstract class EntidadeBase<T>
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public abstract IReadOnlyList<ErroValidacao> Validar();
    public abstract void Atualizar(T entidadeAtualizada);
}
