using MassTransit;
using SharedContracts;

namespace InventoryService.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        Console.WriteLine($"[InventoryService] Received OrderCreated event:");
        Console.WriteLine($"   Order ID: {context.Message.OrderId}");
        Console.WriteLine($"   Product : {context.Message.Product}");
        Console.WriteLine($"   Quantity: {context.Message.Quantity}");

        await File.AppendAllLinesAsync("log.txt", new[]
        {
            $"[InventoryService] OrderCreated event received at {DateTime.Now}:",
            $"   Order ID: {context.Message.OrderId}",
            $"   Product : {context.Message.Product}",
            $"   Quantity: {context.Message.Quantity}",
            ""
        });
    }
}
