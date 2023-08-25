using Microsoft.EntityFrameworkCore;

namespace OrderAPI.Entities;

[Owned]
public class Address
{
    public string Line { get; set; } = null!;
    public string Province { get; set; } = null!;
    public string District { get; set; } = null!;
}