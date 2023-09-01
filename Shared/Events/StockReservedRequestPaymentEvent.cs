using Shared.Interfaces;

namespace Shared.Events;

public class StockReservedRequestPaymentEvent : IStockReservedRequestPaymentEvent
{
    public StockReservedRequestPaymentEvent(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
    public PaymentMessage Payment { get; set; }
    public List<OrderItemMessage> OrderItems { get; set; }
}