using MassTransit;
using SharedContracts;
using InventoryService.Consumers; // Remove if consumer is in this file

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost";
        cfg.Host(host, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Same custom exchange name (must match publisher)
        cfg.Message<OrderCreated>(m =>
        {
            m.SetEntityName("order-events-topic");
        });
        
        cfg.Publish<OrderCreated>(p =>
        {
            p.ExchangeType = "topic";
        });

        // Queue name can be anything – MassTransit will bind it to the custom fanout exchange
        cfg.ReceiveEndpoint("eu-inventory-queue", e =>
        {
            e.ConfigureConsumeTopology = false;
            
            e.Bind<OrderCreated>(b =>
            {
                b.ExchangeType = "topic";
                b.RoutingKey = "order.created.eu";
            });
            
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
    });
});

var app = builder.Build();

app.MapControllers();
app.Run();
