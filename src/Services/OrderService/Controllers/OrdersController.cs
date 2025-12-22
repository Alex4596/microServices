using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SharedContracts;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OrdersController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        // Simulate creating an order ID
        var orderId = new Random().Next(1000, 9999);

        // Publish the event to RabbitMQ
        await _publishEndpoint.Publish(new OrderCreated(orderId, dto.Product, dto.Quantity));

        return Ok(new { Message = "Order created and event published", OrderId = orderId });
    }
}

public class CreateOrderDto
{
    public string Product { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
