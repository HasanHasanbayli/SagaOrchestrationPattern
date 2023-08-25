using Shared.Interfaces;

namespace Shared.Events;

public class OrderCreatedRequestEvent : IOrderCreatedRequestEvent
{
    public Guid OrderId { get; set; }
    public Guid BuyerId { get; set; }
    public List<OrderItemMessage> OrderItems { get; set; } = new();
    public PaymentMessage Payment { get; set; } = new();
}