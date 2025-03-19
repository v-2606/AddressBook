using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQLayer.Interface;

public class EventPublisher: IEventPublisher
{
    private readonly string _hostname = "localhost";
    private readonly string _exchangeName = "event_exchange";
    private readonly string _queueName = "UserRegistrationQueue";

    public void PublishEvent(string routingKey, object eventMessage)
    {
        var factory = new ConnectionFactory() { HostName = _hostname };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct);
        channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: routingKey); 


        var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventMessage));

        channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: routingKey,
            basicProperties: null,
            body: messageBody
        );

        Console.WriteLine($"[Producer] Event Published: {routingKey}");
    }
}
