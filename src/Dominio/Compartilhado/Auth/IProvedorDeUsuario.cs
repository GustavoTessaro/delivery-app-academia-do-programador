namespace DeliveryApp.Dominio.Compartilhado.Auth;

public interface IProvedorDeUsuario
{
    Guid? Id { get; }
    string? Email { get; }
    bool EstaAutenticado { get; }
}
