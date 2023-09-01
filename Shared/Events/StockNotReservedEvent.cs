using Shared.Interfaces;

namespace Shared.Events;

public class StockNotReservedEvent : IStockNotReservedEvent
{
    public StockNotReservedEvent(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
    public string Reason { get; set; } = null!;
}