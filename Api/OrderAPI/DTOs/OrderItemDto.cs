namespace OrderAPI.DTOs;

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
}