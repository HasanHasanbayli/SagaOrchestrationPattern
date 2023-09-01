using Microsoft.EntityFrameworkCore;
using StockAPI.Entities;

namespace StockAPI.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Stock> Stocks { get; set; } = null!;
}