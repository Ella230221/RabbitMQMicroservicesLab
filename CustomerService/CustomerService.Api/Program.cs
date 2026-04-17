using CustomerService.Api.Models;
using CustomerService.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CustomerDb") ?? "Data Source=customers.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    db.Database.EnsureCreated();

    if (!db.Customers.Any())
    {
        db.Customers.AddRange(
            new Customer { Name = "Alice Johnson", Email = "alice@example.com" },
            new Customer { Name = "Bob Smith", Email = "bob@example.com" },
            new Customer { Name = "Carol Lee", Email = "carol@example.com" });
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
