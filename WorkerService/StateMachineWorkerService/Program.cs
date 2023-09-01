using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using StateMachineWorkerService;
using StateMachineWorkerService.Models;

IHost host = Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
{
    services.AddMassTransit(x =>
    {
        x.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>().EntityFrameworkRepository(opt =>
            opt.AddDbContext<DbContext, OrderStateDbContext>((_, builder) =>
            {
                // Add this line before configuring your DbContext
                builder.UseLoggerFactory(LoggerFactory.Create(builder2 => builder2.AddConsole()));

                builder.UseNpgsql(hostContext.Configuration.GetConnectionString("WorkerServiceConnection"), 
                    m =>
                    {
                        m.MigrationsAssembly(typeof(OrderStateDbContext).Assembly.GetName().Name);
                    });
            }));

        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(configure =>
        {
            configure.Host(hostContext.Configuration.GetConnectionString("RabbitMQConnection"));
        
            configure.ReceiveEndpoint(RabbitMqSettingsConst.OrderSaga, endpoint =>
                {
                    endpoint.ConfigureSaga<OrderStateInstance>(provider);
                });
        }));
        
        // x.UsingRabbitMq((context, cfg) =>
        // {
            // cfg.Host(hostContext.Configuration.GetConnectionString("RabbitMQ"));
            
            // cfg.ReceiveEndpoint(RabbitMqSettingsConst.OrderSagaQueue, endpoint =>
            // {
                // endpoint.ConfigureSaga<OrderStateInstance>(context);
            // });

        // });
    });

    services.AddHostedService<Worker>();
    
}).Build();

host.Run();