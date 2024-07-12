using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://teslduyy:ESr7N7TN_h5rGVk75-WpqGkmzTrceyBp@moose.rmq.cloudamqp.com/teslduyy");

var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.BasicQos(0,1,false);

var queueName = channel.QueueDeclare().QueueName;

channel.QueueBind(queueName,"tekrar-fanout","",null);

var consumer = new EventingBasicConsumer(channel);

channel.BasicConsume(queueName,false,consumer);

Console.WriteLine("mesajlar okunuyor...");

consumer.Received += Consumer_Received;

void Consumer_Received(object? sender, BasicDeliverEventArgs e)
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());

    Thread.Sleep(1000);

    Console.WriteLine($"Okunan mesaj - {message}");

    channel.BasicAck(e.DeliveryTag, false);
}

Console.ReadLine();