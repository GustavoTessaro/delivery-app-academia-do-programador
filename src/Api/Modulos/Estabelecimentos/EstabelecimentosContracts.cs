namespace DeliveryApp.WebApi.Modulos.Estabelecimentos;

public sealed record CadastrarEstabelecimentoRequest(
    string NomeComercial,
    string Documento,
    string Endereco,
    string Telefone,
    string AreaAtendimento,
    TimeOnly HorarioAbertura,
    TimeOnly HorarioFechamento,
    decimal TaxaEntrega,
    string Email,
    string Senha
);

public sealed record CadastrarEstabelecimentoResponse(
    Guid Id,
    string NomeComercial
);

public sealed record EstabelecimentoResponse(
    Guid Id,
    string NomeComercial,
    string Documento,
    string Endereco,
    string Telefone,
    string AreaAtendimento,
    TimeOnly HorarioAbertura,
    TimeOnly HorarioFechamento,
    decimal TaxaEntrega,
    bool Ativo
);

public sealed record EditarEstabelecimentoRequest(
    string NomeComercial,
    string Documento,
    string Endereco,
    string Telefone,
    string AreaAtendimento,
    TimeOnly HorarioAbertura,
    TimeOnly HorarioFechamento,
    decimal TaxaEntrega
);

public sealed record AutenticarEstabelecimentoRequest(string Email, string Senha);

public sealed record AutenticarEstabelecimentoResponse(
    Guid EstabelecimentoId,
    string AccessToken,
    DateTime DataExpiracaoEmUtc
);
