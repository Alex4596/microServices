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

        // Set custom exchange name for OrderCreated (fanout by default)
        cfg.Message<OrderCreated>(m =>
        {
            m.SetEntityName("custom-ordercreated-fanout");
        });
    });
});

var app = builder.Build();

app.MapControllers();
app.Run();
