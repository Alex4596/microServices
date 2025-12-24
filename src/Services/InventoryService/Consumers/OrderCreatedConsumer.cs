using MassTransit;
using SharedContracts;

namespace InventoryService.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    public Task Consume(ConsumeContext<OrderCreated> context)
    {
        Console.WriteLine("=== [EU InventoryService] Received matching OrderCreated ===");
        Console.WriteLine($"Region via routing key match: {context.Message.Region}");
        Console.WriteLine($"Order ID: {context.Message.OrderId}, Product: {context.Message.Product}, Quantity: {context.Message.Quantity}");
        return Task.CompletedTask;
    }
}
