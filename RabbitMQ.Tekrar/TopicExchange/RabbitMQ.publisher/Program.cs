using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://teslduyy:ESr7N7TN_h5rGVk75-WpqGkmzTrceyBp@moose.rmq.cloudamqp.com/teslduyy");

var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.ExchangeDeclare("tekrar-topic-exchange",ExchangeType.Topic,true,false);

Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    var log1 = (MessageNames)new Random().Next(1, 5);
    var log2 = (MessageNames)new Random().Next(1, 5);
    var log3 = (MessageNames)new Random().Next(1, 5);

    var route = $"{log1}.{log2}.{log3}";

    var message = $"{x}. mesaj : {log1}.{log2}.{log3}";

    var messageBody = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish("tekrar-topic-exchange",route,null, messageBody);

    Console.WriteLine(message);
});
Console.Read();



public enum MessageNames
{
    Critical = 1,
    Error = 2,
    Warning = 3,
    Info = 4
}