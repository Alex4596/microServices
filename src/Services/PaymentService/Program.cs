using MassTransit;
using PaymentService.Consumers;
using SharedContracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("eu-creditcard-payments", e =>
        {
            e.ConfigureConsumeTopology = false;

            e.Bind<OrderPaid>(b =>
            {
                b.ExchangeType = "headers";
                b.SetBindingArgument("currency", "EUR");
                b.SetBindingArgument("payment_method", "credit_card");
                b.SetBindingArgument("status", "completed");
                b.SetBindingArgument("x-match", "all");
            });

            e.ConfigureConsumer<PaymentConsumer>(context);
        });
    });
});

var app = builder.Build();
app.Run();
