namespace DeliveryApp.WebApi.Compartilhado.Logging;

public sealed class NewRelicOptions
{
    public const string SectionName = "NewRelic";

    public bool Enabled { get; set; }
    public string EndpointUrl { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = string.Empty;
    public string? LicenseKey { get; set; }
}
