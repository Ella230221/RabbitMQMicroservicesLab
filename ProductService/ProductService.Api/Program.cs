using Microsoft.EntityFrameworkCore;
using ProductService.Api.Data;
using ProductService.Api.Models;
using ProductService.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ProductDb") ?? "Data Source=products.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<OrderCreatedConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.EnsureCreated();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new Product { Name = "Laptop", Price = 1299.99m, Stock = 8 },
            new Product { Name = "Phone", Price = 699.00m, Stock = 15 },
            new Product { Name = "Headphones", Price = 149.50m, Stock = 25 });
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
