namespace OrderAPI.DTOs;

public class AddressDto
{
    public string Line { get; set; } = null!;
    public string Province { get; set; } = null!;
    public string District { get; set; } = null!;
}