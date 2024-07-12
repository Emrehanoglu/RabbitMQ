using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://teslduyy:ESr7N7TN_h5rGVk75-WpqGkmzTrceyBp@moose.rmq.cloudamqp.com/teslduyy");

var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.ExchangeDeclare("tekrar-fanout", ExchangeType.Fanout, true, false);

Enumerable.Range(1, 25).ToList().ForEach(x =>
{
    var message = $"mesaj - {x}";
    var messageBody = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish("tekrar-fanout","",null,messageBody);

    Console.WriteLine($"Mesaj gönderilmiştir - {message}");
});
Console.ReadLine();