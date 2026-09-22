namespace DeliveryApp.Aplicacao.Modulos.Estabelecimentos.DTOs;

public sealed record EstabelecimentoDto(
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
