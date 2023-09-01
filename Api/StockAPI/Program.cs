using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using StockAPI.Consumers;
using StockAPI.Context;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;
IConfiguration configuration = builder.Configuration;

// Add services to the container.

services.AddControllers();

services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
    optionsBuilder.UseNpgsql(configuration.GetConnectionString(name: "SqlConnection"))
);

services.AddMassTransit(configure =>
{
    configure.AddConsumer<OrderCreatedEventConsumer>();

    configure.UsingRabbitMq(configure: (context, cfg) =>
    {
        cfg.Host(configuration.GetConnectionString(name: "RabbitMQConnection"));
        cfg.ReceiveEndpoint(RabbitMqSettingsConst.OrderSaga,
            e =>
            {
                e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
            });
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();