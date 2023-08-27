using MassTransit;
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

        Event(() => OrderCreateRequestEvent, x =>
            x.CorrelateById(context =>
                context.OrderId, context => context.Message.OrderId).SelectId(_ => Guid.NewGuid()));

        Initially(When(OrderCreateRequestEvent).Then(context =>
            {
                context.Instance.BuyerId = context.Data.BuyerId;
                context.Instance.OrderId = context.Data.OrderId;
                context.Instance.CreatedDate = DateTime.Now;

                context.Instance.CardName = context.Data.Payment.CardName;
                context.Instance.CardNumber = context.Data.Payment.CardNumber;
                context.Instance.Expiration = context.Data.Payment.Expiration;
                context.Instance.Cvv = context.Data.Payment.Cvv;
                context.Instance.Expiration = context.Data.Payment.Expiration;
                context.Instance.TotalPrice = context.Data.Payment.TotalPrice;
            }).Then(context => { Console.WriteLine($"OrderCreatedRequestEvent before: {context.Instance}"); })
            .TransitionTo(OrderCreated).Then(context =>
            {
                Console.WriteLine($"OrderCreatedRequestEvent after: {context.Instance}");
            }));
    }
}