using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentService.Services;

public class PaymentCreatedConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentCreatedConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public PaymentCreatedConsumer(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<PaymentCreatedConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
            UserName = "guest",
            Password = "guest"
        };

        var exchangeName = _configuration["RabbitMQ:ExchangeName"] ?? "orders-exchange";
        var queueName = _configuration["RabbitMQ:QueueName"] ?? "payment-orders-queue";

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(
                    exchange: exchangeName,
                    type: ExchangeType.Fanout,
                    durable: true,
                    autoDelete: false,
                    arguments: null);
                _channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                _channel.QueueBind(queueName, exchangeName, string.Empty);
                _channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (_, ea) =>
                {
                    try
                    {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var orderCreated = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

                        if (orderCreated is null)
                        {
                            _channel.BasicNack(ea.DeliveryTag, false, false);
                            return;
                        }

                        using var scope = _serviceProvider.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

                        var alreadyExists = await dbContext.Payments
                            .AnyAsync(payment => payment.OrderId == orderCreated.OrderId, stoppingToken);

                        if (!alreadyExists)
                        {
                            dbContext.Payments.Add(new Payment
                            {
                                OrderId = orderCreated.OrderId,
                                CustomerId = orderCreated.CustomerId,
                                Amount = orderCreated.TotalPrice,
                                Status = "Processed",
                                ProcessedAtUtc = DateTime.UtcNow
                            });

                            await dbContext.SaveChangesAsync(stoppingToken);
                        }

                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process payment event.");
                        _channel?.BasicNack(ea.DeliveryTag, false, true);
                    }
                };

                _channel.BasicConsume(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer);

                _logger.LogInformation("PaymentService connected to RabbitMQ and is consuming {QueueName}.", queueName);
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "PaymentService could not connect to RabbitMQ yet. Retrying in 5 seconds.");
                _channel?.Dispose();
                _connection?.Dispose();
                _channel = null;
                _connection = null;
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
