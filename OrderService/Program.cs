using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("OrderDb") ?? "Data Source=orders.db"));

builder.Services.AddControllers();
builder.Services.AddHttpClient("Customers", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceEndpoints:CustomerService"]
        ?? "http://localhost:5001");
});
builder.Services.AddHttpClient("Products", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceEndpoints:ProductService"]
        ?? "http://localhost:5003");
});
builder.Services.AddSingleton(_ => new RabbitMQPublisher(
    builder.Configuration["RabbitMQ:HostName"] ?? "localhost",
    builder.Configuration["RabbitMQ:ExchangeName"] ?? "orders-exchange"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
