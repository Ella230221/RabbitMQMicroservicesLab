using RabbitMQ.Client;
using System.Text;

namespace OrderService.Services
{
    public class RabbitMQPublisher
    {
        private readonly string _hostname;
        private readonly string _exchangeName;

        public RabbitMQPublisher(string hostname, string exchangeName)
        {
            _hostname = hostname;
            _exchangeName = exchangeName;
        }

        public void Publish(string message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostname,
                UserName = "guest",
                Password = "guest"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null
            );

            var body = Encoding.UTF8.GetBytes(message);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: string.Empty,
                basicProperties: properties,
                body: body
            );

            Console.WriteLine($"Message published to exchange: {_exchangeName}");
        }
    }
}
