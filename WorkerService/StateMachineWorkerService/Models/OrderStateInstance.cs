using System.Text;
using MassTransit;

namespace StateMachineWorkerService.Models;

public class OrderStateInstance : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public Guid BuyerId { get; set; }
    public Guid OrderId { get; set; }

    public string CardName { get; set; } = null!;
    public string CardNumber { get; set; } = null!;
    public string Expiration { get; set; } = null!;
    public string Cvv { get; set; } = null!;
    public decimal TotalPrice { get; set; }

    public DateTime CreatedDate { get; set; }

    public override string ToString()
    {
        var properties = GetType().GetProperties();
        
        StringBuilder sb = new();
         
        properties.ToList().ForEach(p =>
        {
            sb.Append($"{p.Name}: {p.GetValue(this)}\n");
        });
        
        sb.Append('\n');
        
        return sb.ToString();
    }
}