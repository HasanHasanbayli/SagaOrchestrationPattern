using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Events;
using Shared.Interfaces;
using StockAPI.Context;
using StockAPI.Entities;

namespace StockAPI.Consumers;

public class OrderCreatedEventConsumer : IConsumer<IOrderCreatedEvent>
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<OrderCreatedEventConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderCreatedEventConsumer(ApplicationDbContext applicationDbContext,
        ILogger<OrderCreatedEventConsumer> logger,
        IPublishEndpoint publishEndpoint)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IOrderCreatedEvent> context)
    {
        var stockResult = new List<bool>();

        foreach (OrderItemMessage item in context.Message.OrderItems)
        {
            stockResult.Add(await _applicationDbContext.Stocks.AnyAsync(stock =>
                stock.ProductId == item.ProductId && stock.Count > item.Count));
        }

        if (stockResult.All(x => x.Equals(true)))
        {
            foreach (OrderItemMessage item in context.Message.OrderItems)
            {
                Stock? stock = await _applicationDbContext.Stocks.FirstOrDefaultAsync(
                    stock => stock.ProductId == item.ProductId);

                if (stock != null)
                {
                    stock.Count -= item.Count;
                }

                await _applicationDbContext.SaveChangesAsync();
            }

            _logger.LogInformation($"Stock for reserved for CorrelationId Id: {context.Message.CorrelationId}");

            StockReservedEvent stockReservedEvent = new(context.Message.CorrelationId)
            {
                OrderItems = context.Message.OrderItems
            };

            await _publishEndpoint.Publish(stockReservedEvent);
        }
        else
        {
            await _publishEndpoint.Publish(new StockNotReservedEvent(context.Message.CorrelationId)
            {
                Reason = "Not enough stock"
            });

            _logger.LogInformation($"Not enough stock for CorrelationId Id: {context.Message.CorrelationId}");
        }
    }
}