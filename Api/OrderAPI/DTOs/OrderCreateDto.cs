namespace OrderAPI.DTOs;

public class OrderCreateDto
{
    public Guid BuyerId { get; set; }
    public List<OrderItemDto> OrderItem { get; set; } = new();
    public PaymentDto Payment { get; set; } = null!;
    public AddressDto Address { get; set; } = null!;
}