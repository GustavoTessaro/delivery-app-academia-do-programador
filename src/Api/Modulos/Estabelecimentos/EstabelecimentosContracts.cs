namespace DeliveryApp.WebApi.Modulos.Estabelecimentos;

public sealed record CadastrarEstabelecimentoRequest(
    string NomeComercial,
    string Documento,
    string Endereco,
    string Telefone,
    string AreaAtendimento,
    TimeOnly HorarioAbertura,
    TimeOnly HorarioFechamento,
    string Email,
    string Senha
);

public sealed record CadastrarEstabelecimentoResponse(
    Guid Id,
    string NomeComercial
);

public sealed record AutenticarEstabelecimentoRequest(string Email, string Senha);

public sealed record AutenticarEstabelecimentoResponse(
    Guid EstabelecimentoId,
    string AccessToken,
    DateTime DataExpiracaoEmUtc
);

