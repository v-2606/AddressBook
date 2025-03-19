using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;

public class RabbitMQConsumer : BackgroundService
{
    private readonly string _hostname = "localhost";
    private readonly string _exchangeName = "event_exchange";
    private readonly string _queueName = "UserRegistrationQueue"; 
    private readonly string _routingKey = "user.registered";
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = _hostname };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct);
        channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: _routingKey);


        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: "user.registered");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"[Consumer] Received Event: {message}");

 
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }
   
    
}
