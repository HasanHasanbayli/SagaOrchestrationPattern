using MassTransit;
using Shared;
using Shared.Events;
using Shared.Interfaces;

namespace StateMachineWorkerService.Models;

public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
{
    public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; }
    public Event<IStockReservedEvent> StockReservedEvent { get; set; }
    public State OrderCreated { get; }
    public State StockReserved { get; }

    [Obsolete("Obsolete")]
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCreatedRequestEvent, y =>
            y.CorrelateBy<Guid>(x =>
                x.OrderId, z => z.Message.OrderId).SelectId(_ => Guid.NewGuid()));

        Initially(When(OrderCreatedRequestEvent)
            .Then(context =>
            {
                context.Instance.BuyerId = context.Data.BuyerId;
                context.Instance.OrderId = context.Data.OrderId;
                context.Instance.CreatedDate = DateTime.UtcNow;
                context.Instance.CardName = context.Data.Payment.CardName;
                context.Instance.CardNumber = context.Data.Payment.CardNumber;
                context.Instance.Expiration = context.Data.Payment.Expiration;
                context.Instance.Cvv = context.Data.Payment.Cvv;
                context.Instance.Expiration = context.Data.Payment.Expiration;
                context.Instance.TotalPrice = context.Data.Payment.TotalPrice;
            })
            .Then(context => { Console.WriteLine($"OrderCreatedRequestEvent before: {context.Instance}"); })
            .Publish(context => new OrderCreatedEvent(context.Instance.CorrelationId)
            {
                OrderItems = context.Data.OrderItems
            })
            .TransitionTo(OrderCreated).Then(context =>
            {
                Console.WriteLine($"OrderCreatedRequestEvent after: {context.Instance}");
            }));


        During(OrderCreated,
            When(StockReservedEvent)
                .TransitionTo(StockReserved)
                .Send(new Uri($"queue: {RabbitMqSettingsConst.PaymentStockReservedRequestQueueName}"),
                    context => new StockReservedRequestPaymentEvent(context.Instance.CorrelationId)
                    {
                        OrderItems = context.Message.OrderItems,
                        Payment = new PaymentMessage
                        {
                            CardName = context.Instance.CardName,
                            CardNumber = context.Instance.CardNumber,
                            Cvv = context.Instance.Cvv,
                            Expiration = context.Instance.Expiration,
                            TotalPrice = context.Instance.TotalPrice,
                        }
                    }).Then(context => { Console.WriteLine($"StockReservedEvent after: {context.Instance}"); }));
    }
}