using Microsoft.AspNetCore.Mvc;
using MassTransit;
using SharedContracts;
using System.Threading;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private static int _nextId = 1;

    public OrdersController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    // Existing: Create Order
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var orderCreated = new OrderCreated(
            OrderId: Interlocked.Increment(ref _nextId),
            Product: request.Product,
            Quantity: request.Quantity,
            Region: request.Region.ToUpper()
        );

        var routingKey = $"order.created.{orderCreated.Region.ToLowerInvariant()}";
        
        await _publishEndpoint.Publish(orderCreated, context =>
        {
            context.SetRoutingKey(routingKey);
        });

        return Ok(new
        {
            message = "Order created and event published",
            orderId = orderCreated.OrderId
        });
    }

    // NEW: Pay Order
    [HttpPost("{id}/pay")]
    public async Task<IActionResult> PayOrder(int id, [FromBody] PayOrderRequest request)
    {
        var orderPaid = new OrderPaid(
            OrderId: id,
            Amount: request.Amount,
            Currency: request.Currency.ToUpper(),
            PaymentMethod: request.PaymentMethod.ToLower(),
            Status: request.Status.ToLower()
        );

        await _publishEndpoint.Publish(orderPaid, context =>
        {
            context.Headers.Set("currency", orderPaid.Currency);
            context.Headers.Set("payment_method", orderPaid.PaymentMethod);
            context.Headers.Set("status", orderPaid.Status);
        });

        return Ok(new
        {
            message = "Payment processed and OrderPaid event published with headers",
            orderId = orderPaid.OrderId,
            amount = orderPaid.Amount,
            currency = orderPaid.Currency,
            method = orderPaid.PaymentMethod,
            status = orderPaid.Status
        });
    }
}

public record CreateOrderRequest(
    string Product,
    int Quantity,
    string Region
    );
public record PayOrderRequest(
    decimal Amount,
    string Currency,
    string PaymentMethod = "credit_card",
    string Status = "completed"
    );