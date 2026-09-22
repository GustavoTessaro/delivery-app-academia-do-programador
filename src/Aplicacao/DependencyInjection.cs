using DeliveryApp.Aplicacao.Modulos.Pedidos.Mensageria;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryApp.Aplicacao;

public static class DependencyInjection
{
    public static void AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        var rabbitMqConnectionString = configuration.GetConnectionString("RabbitMq")
            ?? throw new InvalidOperationException("A ConnectionString \"RabbitMq\" não foi configurada");

        services.AddMassTransit(config =>
        {
            config.AddConsumer<CriarPedidoConsumer>();

            config.UsingRabbitMq((context, rabbitMq) =>
            {
                rabbitMq.Host(new Uri(rabbitMqConnectionString));

                rabbitMq.ReceiveEndpoint("pedidos-criados", endpoint =>
                {
                    endpoint.PrefetchCount = 4;
                    endpoint.ConcurrentMessageLimit = 2;

                    endpoint.ConfigureConsumer<CriarPedidoConsumer>(context);
                });
            });
        });

        services.Configure<MassTransitHostOptions>(options =>
        {
            options.WaitUntilStarted = true;
            options.StartTimeout = TimeSpan.FromSeconds(30);
        });
    }
}
