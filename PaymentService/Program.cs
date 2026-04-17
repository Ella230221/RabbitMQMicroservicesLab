using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;
using PaymentService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PaymentDb") ?? "Data Source=payments.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<PaymentCreatedConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
    db.Database.EnsureCreated();

    if (!db.Payments.Any())
    {
        db.Payments.Add(new Payment
        {
            OrderId = 0,
            CustomerId = 0,
            Amount = 0,
            Status = "ServiceReady",
            ProcessedAtUtc = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
