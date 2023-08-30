using Shared.Interfaces;

namespace Shared.Events;

public class OrderCreatedEvent : IOrderCreatedEvent
{
    public Guid CorrelationId { get; }
    public List<OrderItemMessage> OrderItems { get; set; } = new();
}