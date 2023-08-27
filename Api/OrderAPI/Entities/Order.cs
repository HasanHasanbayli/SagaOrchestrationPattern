using OrderAPI.Enums;

namespace OrderAPI.Entities;

public class Order
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid BuyerId { get; set; }
    public Address Address { get; set; } = null!;
    public OrderStatus Status { get; set; }
    public string? FailMessage { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}