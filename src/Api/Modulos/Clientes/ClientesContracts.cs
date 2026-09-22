namespace DeliveryApp.WebApi.Modulos.Clientes;

public sealed record CadastrarClienteRequest(
    string Nome,
    string Cpf,
    string Email,
    string Senha
);

public sealed record AutenticarClienteRequest(string Email, string Senha);

public sealed record AutenticacaoClienteResponse(
    Guid ClienteId,
    string AccessToken,
    DateTime DataExpiracaoEmUtc
);

public sealed record ClienteResponse(Guid Id, string Nome, string Cpf, string Email);
