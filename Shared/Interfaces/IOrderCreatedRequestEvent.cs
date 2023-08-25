namespace Shared.Interfaces;

public interface IOrderCreatedRequestEvent
{
    public Guid OrderId { get; set; }
    public Guid BuyerId { get; set; }
    public List<OrderItemMessage> OrderItems { get; set; }
    public PaymentMessage Payment { get; set; }
}