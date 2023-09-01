using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Context;
using OrderAPI.DTOs;
using OrderAPI.Entities;
using OrderAPI.Enums;
using Shared;
using Shared.Events;
using Shared.Interfaces;

namespace OrderAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public OrderController(ApplicationDbContext context, ISendEndpointProvider sendEndpointProvider)
    {
        _context = context;
        _sendEndpointProvider = sendEndpointProvider;
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderCreateDto orderCreateDto)
    {
        Order newOrder = new()
        {
            BuyerId = orderCreateDto.BuyerId,
            Status = OrderStatus.InProgress,
            Address = new Address
            {
                Line = orderCreateDto.Address.Line,
                Province = orderCreateDto.Address.Province,
                District = orderCreateDto.Address.District
            },
            CreatedDate = DateTime.UtcNow
        };
  
        orderCreateDto.OrderItem.ForEach(dto =>
        {
            newOrder.Items.Add(new OrderItem
            {
                Price = dto.Price,
                ProductId = dto.ProductId,
                Count = dto.Count
            });
        });

        await _context.Orders.AddAsync(newOrder);

        await _context.SaveChangesAsync();

        decimal totalPrice = newOrder.Items.Sum(orderItem => orderItem.Price * orderItem.Count);

        OrderCreatedRequestEvent orderCreatedRequestEvent = new()
        {
            BuyerId = orderCreateDto.BuyerId,
            OrderId = newOrder.Id,
            Payment = new PaymentMessage
            {
                CardName = orderCreateDto.Payment.CardName,
                CardNumber = orderCreateDto.Payment.CardNumber,
                Expiration = orderCreateDto.Payment.Expiration,
                Cvv = orderCreateDto.Payment.Cvv,
                TotalPrice = totalPrice
            }
        };

        orderCreateDto.OrderItem.ForEach(dto =>
        {
            orderCreatedRequestEvent.OrderItems.Add(new OrderItemMessage
            {
                Count = dto.Count,
                ProductId = dto.ProductId
            });
        });

        ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(
        address: new Uri($"queue:{RabbitMqSettingsConst.OrderSaga}"));

        await sendEndpoint.Send<IOrderCreatedRequestEvent>(orderCreatedRequestEvent);

        return Ok();
    }
}