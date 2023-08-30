using MassTransit;
using Shared.Events;
using Shared.Interfaces;

namespace StateMachineWorkerService.Models;

public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
{
    public Event<IOrderCreatedRequestEvent> OrderCreateRequestEvent { get; set; } = null!;
    public State OrderCreated { get; private set; } = null!;

    [Obsolete("Obsolete")]
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCreateRequestEvent, y =>
            y.CorrelateBy<Guid>(x =>
                x.OrderId, z => z.Message.OrderId).SelectId(_ => Guid.NewGuid()));

        Initially(When(OrderCreateRequestEvent).Then(context =>
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
            }).Then(context =>
            {
                Console.WriteLine($"OrderCreatedRequestEvent before: {context.Instance}");
            }).Publish(context=>new OrderCreatedEvent{OrderItems = context.Data.OrderItems})
            .TransitionTo(OrderCreated).Then(context =>
            {
                Console.WriteLine($"OrderCreatedRequestEvent after: {context.Instance}");
            }));
    }
}