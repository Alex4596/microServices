using MassTransit;
using SharedContracts;
using InventoryService.Consumers;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore; // Remove if a consumer is in this file

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseInMemoryDatabase("InventoryDb"));

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
        /*
        cfg.Message<OrderCreated>(m =>
        {
            m.SetEntityName("order-events-topic");
        });
        
        
        cfg.Publish<OrderCreated>(p =>
        {
            p.ExchangeType = "topic";
        });
        */

        // Queue name can be anything – MassTransit will bind it to the custom fanout exchange
        cfg.ReceiveEndpoint("eu-inventory-queue", e =>
        {
            e.ConfigureConsumeTopology = false; // required for custom bindings
            
            e.Bind<OrderCreated>(b =>
            {
                b.ExchangeType = "topic"; // we want topic routing
                b.RoutingKey = "order.created.eu"; // bind only EU orders
            });
            
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapControllers();
app.Run();
