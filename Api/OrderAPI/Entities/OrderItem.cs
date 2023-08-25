namespace OrderAPI.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int Count { get; set; }
}