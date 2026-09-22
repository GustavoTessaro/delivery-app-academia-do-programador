using DeliveryApp.Dominio.Compartilhado.Auth;

namespace DeliveryApp.Dominio.Modulos.Pedidos;

public sealed class TransicaoStatusPedido
{
    public const int TamanhoMaximoMotivo = 500;

    public Guid Id { get; private set; }
    public Guid PedidoId { get; private set; }
    public Guid UsuarioId { get; private set; }
    public TipoUsuario TipoUsuario { get; private set; }

    public StatusPedido? StatusAnterior { get; private set; }
    public StatusPedido StatusAtual { get; private set; }
    public string? Motivo { get; private set; }
    public DateTimeOffset OcorridaEmUtc { get; private set; }

    private TransicaoStatusPedido() { }

    public TransicaoStatusPedido(
        Guid usuarioId,
        TipoUsuario tipoUsuario,
        StatusPedido? statusAnterior,
        StatusPedido statusAtual,
        string? motivo,
        DateTimeOffset ocorridaEmUtc
    )
    {
        Id = Guid.CreateVersion7();
        UsuarioId = usuarioId;
        TipoUsuario = tipoUsuario;
        StatusAnterior = statusAnterior;
        StatusAtual = statusAtual;
        Motivo = string.IsNullOrWhiteSpace(motivo) ? null : motivo.Trim();
        OcorridaEmUtc = ocorridaEmUtc;
    }
}
