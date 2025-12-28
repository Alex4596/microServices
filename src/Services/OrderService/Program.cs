using MassTransit;
using SharedContracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Configure MassTransit
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost";
        cfg.Host(host, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        
        cfg.Publish<OrderCreated>(p =>
        {
            p.ExchangeType = "topic";
            p.AutoDelete = false;
            p.Durable = true;
        });
        
        cfg.Publish<OrderPaid>(p =>
        {
            p.ExchangeType = "headers";
            p.AutoDelete = false;
            p.Durable = true;
        });

        // Set custom exchange name for OrderCreated (fanout by default)
        /*
        cfg.Message<OrderCreated>(m =>
        {
            m.SetEntityName("order-events-topic");
        });
        */
        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapControllers();
app.Run();
