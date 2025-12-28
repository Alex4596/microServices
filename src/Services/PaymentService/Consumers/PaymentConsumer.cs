using MassTransit;
using SharedContracts;

namespace PaymentService.Consumers;

public class PaymentConsumer : IConsumer<OrderPaid>
{
    private readonly ILogger<PaymentConsumer> _logger;

    public PaymentConsumer(ILogger<PaymentConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderPaid> context)
    {
        var msg = context.Message;

        _logger.LogInformation(
            "💳 [PAYMENT SERVICE] Received successful EUR credit card payment!\n" +
            "Order {OrderId} | Amount: {Amount} {Currency} | Status: {Status}",
            msg.OrderId, msg.Amount, msg.Currency, msg.Status);

        return Task.CompletedTask;
    }
}
