using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace DeliveryApp.WebApi.Compartilhado.Logging;

public static class LoggingExtensions
{
    public static void AddSerilogServices(
        this IServiceCollection services,
        ILoggingBuilder logging
    )
    {
        // Remove o provedor padrão de logs da Microsoft e adiciona Serilog
        logging.ClearProviders();
        services.AddSerilog((serviceProvider, loggerConfiguration) =>
        {
            NewRelicOptions options = serviceProvider
                .GetRequiredService<IOptions<NewRelicOptions>>()
                .Value;

            ConfigurarLogger(loggerConfiguration, options);
        });
    }

    private static void ConfigurarLogger(
        LoggerConfiguration loggerConfiguration,
        NewRelicOptions newRelicOptions
    )
    {
        string caminhoAppData = Environment
            .GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string caminhoDiretorio = Path.Combine(caminhoAppData, "DeliveryApp");

        Directory.CreateDirectory(caminhoDiretorio);

        string caminhoLogs = Path.Combine(caminhoDiretorio, "erro.log");

        loggerConfiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                caminhoLogs,
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Error
            );

        if (newRelicOptions.Enabled)
        {
            if (string.IsNullOrWhiteSpace(newRelicOptions.LicenseKey))
            {
                throw new InvalidOperationException(
                    "A chave de licença do New Relic não foi configurada. Configure Infra:NewRelic:LicenseKey."
                );
            }

            loggerConfiguration.WriteTo.NewRelicLogs(
                endpointUrl: newRelicOptions.EndpointUrl,
                applicationName: newRelicOptions.ApplicationName,
                licenseKey: newRelicOptions.LicenseKey
            );
        }
    }
}
