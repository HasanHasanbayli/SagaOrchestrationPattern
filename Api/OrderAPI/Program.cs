using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Context;

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
    configure.UsingRabbitMq(configure: (_, cfg) =>
    {
        cfg.Host(configuration.GetConnectionString(name: "RabbitMQConnection"));
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