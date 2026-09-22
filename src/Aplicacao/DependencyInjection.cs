using Microsoft.Extensions.DependencyInjection;

namespace DeliveryApp.Aplicacao;

public static class DependencyInjection
{
    public static void AddApplicationServices(
        this IServiceCollection services
    )
    {
        using var serviceProvider = services.BuildServiceProvider();
    }
}
