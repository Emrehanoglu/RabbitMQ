using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://teslduyy:ESr7N7TN_h5rGVk75-WpqGkmzTrceyBp@moose.rmq.cloudamqp.com/teslduyy");

var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.ExchangeDeclare("tekrar-direct-exchange",ExchangeType.Direct,true,false);

Enum.GetNames(typeof(MessageNames)).ToList().ForEach(x =>
{
    var queueName = $"tekrar-direct-queue-{x}";
    channel.QueueDeclare(queueName,true,false,false);
    
    var routeKey = $"route-{x}";

    channel.QueueBind(queueName, "tekrar-direct-exchange",routeKey);
});

Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    MessageNames messageType = (MessageNames)new Random().Next(1,5);

    var message = $"{x}. message-{messageType}";
    var messageBody = Encoding.UTF8.GetBytes(message);

    var route = $"route-{messageType}";

    channel.BasicPublish("tekrar-direct-exchange",route,null, messageBody);
    Console.WriteLine($"Message sended : {message}");
});
Console.ReadLine();

public enum MessageNames
{
    Critical = 1,
    Error = 2,
    Warning = 3,
    Info = 4
}