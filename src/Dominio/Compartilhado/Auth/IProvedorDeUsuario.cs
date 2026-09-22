namespace DeliveryApp.Dominio.Compartilhado.Auth;

public interface IProvedorDeUsuario
{
    Guid? Id { get; }
    bool EstaAutenticado { get; }
}
