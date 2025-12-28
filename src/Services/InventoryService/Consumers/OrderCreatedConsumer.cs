using MassTransit;
using SharedContracts;

namespace InventoryService.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderCreated> context)
    {
        var msg = context.Message;

        _logger.LogInformation("Funguje! Received OrderCreated event");

        if (msg.Region.Equals("EU", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation(
                "=== [EU InventoryService] Received matching Order ===\n" +
                "Order ID: {OrderId}, Product: {Product}, Quantity: {Quantity}",
                msg.OrderId, msg.Product, msg.Quantity);
        }

        return Task.CompletedTask;
    }
}
