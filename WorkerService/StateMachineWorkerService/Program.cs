using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using StateMachineWorkerService;
using StateMachineWorkerService.Models;

IHost host = Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
{
    services.AddMassTransit(cfg =>
    {
        cfg.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>().EntityFrameworkRepository(opt =>
            opt.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
            {
                builder.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"), m =>
                    {
                        m.MigrationsAssembly(typeof(OrderStateDbContext).Assembly.GetName().Name);
                    });
            }));

        cfg.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(configure =>
        {
            configure.Host(hostContext.Configuration.GetConnectionString("RabbitMQ"));

            configure.ReceiveEndpoint(RabbitMqSettingsConst.OrderSagaQueue, endpoint =>
                {
                    endpoint.ConfigureSaga<OrderStateInstance>(provider);
                });
        }));
    });

    services.AddHostedService<Worker>();
}).Build();

host.Run();