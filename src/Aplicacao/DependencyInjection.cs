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
            config.AddConsumer<AlterarStatusPedidoConsumer>();

            config.UsingRabbitMq((context, rabbitMq) =>
            {
                rabbitMq.Host(new Uri(rabbitMqConnectionString));

                rabbitMq.ReceiveEndpoint("pedidos-criados", endpoint =>
                {
                    endpoint.PrefetchCount = 4;
                    endpoint.ConcurrentMessageLimit = 2;
                    endpoint.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    endpoint.ConfigureConsumer<CriarPedidoConsumer>(context);
                });

                rabbitMq.ReceiveEndpoint("pedidos-atualizados", endpoint =>
                {
                    endpoint.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    endpoint.ConfigureConsumer<AlterarStatusPedidoConsumer>(context);
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
