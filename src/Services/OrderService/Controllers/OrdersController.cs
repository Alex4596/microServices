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
        var orderId = new Random().Next(1000, 9999);

        var routingKey = $"order.created.{dto.Region.ToLower()}";

        await _publishEndpoint.Publish(
            new OrderCreated(orderId, dto.Product, dto.Quantity, dto.Region),
            context => context.SetRoutingKey(routingKey)
            );

        return Ok(new { Message = $"Order published with routing key: {routingKey}", OrderId = orderId });
    }
}

public class CreateOrderDto
{
    public required string Product { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Region { get; set; } = "eu";
}
